using System;

abstract class Expression : Symbol
{
    public abstract string GetName();
    public abstract int GetLine();

    public abstract T Accept<T>(Visitor<T> visitor);

    public interface Visitor<T>
    {
        T visitBinaryExpr(Expression.Binary expr);
        T visitUnaryExpr(Expression.Unary expr);
        T visitOperandExpr(Expression.Operand expr);
    }
    public abstract string Type(Environment env);

    public class Binary : Expression
    {
        public Binary(Operand first, Token op, Operand second)
        {
            First = first;
            Operator = op;
            Second = second;
        }
        public Operand First { get; set; }
        public Token Operator { get; }
        public Operand Second { get; set; }
        public override string GetName()
        {
            return "BINARY";
        }

        public override int GetLine()
        {
            return First.GetLine();
        }

        public override string ToString()
        {
            return string.Format("(expr {1} {0} {2})", First, Operator, Second);
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitBinaryExpr(this);
        }

        public override string Type(Environment environment)
        {
            var firstType = First.Type(environment);
            var secondType = Second.Type(environment);

            if (firstType == null || secondType == null)
            {
                return null;
            }

            if (firstType != secondType)
            {
                Console.WriteLine(string.Format("Cannot operate with {0} and {1}", firstType, secondType));
                return null;
            }

            // TODO use eval to get types?
            if (
                  (Operator.Type == TokenType.EQUAL ||
                   Operator.Type == TokenType.AND ||
                   Operator.Type == TokenType.NOT))
            {

                return "bool";
            }

            return First.Type(environment);
        }
    }

    public class Unary : Expression
    {
        public Unary(Token op, Operand operand)
        {
            Operand = operand;
            Operator = op;
        }
        // TODO rename this
        public Operand Operand { get; set; }
        public Token Operator { get; set; }

        public override string GetName()
        {
            return "UNARY";
        }

        public override int GetLine()
        {
            return Operand.GetLine();
        }

        public override string ToString()
        {
            return string.Format("(expr {0} {1})", Operator, Operand);
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitUnaryExpr(this);
        }

        public override string Type(Environment environment)
        {
            var firstType = Operand.Type(environment);

            // TODO use eval to get types?
            if (Operator != null && Operator.Type == TokenType.NOT)
            {

                return "bool";
            }

            return firstType;
        }
    }

    public class Operand : Expression
    {
        public Operand(Symbol value)
        {
            Value = value;
        }

        public Symbol Value { get; }

        public override string GetName()
        {
            return "OPERAND";
        }

        public override string ToString()
        {
            return string.Format("(opnd {0})", Value.ToString());
        }

        public override int GetLine()
        {
            return Value.GetLine();
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitOperandExpr(this);
        }

        public override string Type(Environment environment)
        {
            switch (Value.GetName())
            {
                case "UNARY":
                case "BINARY":
                    return ((Expression)Value).Type(environment);
                case "INTEGER":
                    return "int";
                case "STRING":
                    return "string";
                case "VAR_IDENTIFIER":
                    var ident = (VarIdentifier)Value;

                    if (!environment.Contains(ident.Name))
                    {
                        Console.WriteLine(string.Format("Cannot operate on uninitalized variable '{0}'", ident.Name));
                        return null;
                    }

                    return environment.GetType(ident.Name);
                default:
                    return null;
            }
        }
    }
}
