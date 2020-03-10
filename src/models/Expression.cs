using System;

abstract class Expression : Symbol
{
    public abstract string GetName();
    public abstract int GetLine();
    public abstract object Eval(Environment env);
    public abstract string Type(Environment env);

    public class Binary : Expression
    {
        public Binary(Operand first, Token op, Operand second)
        {
            First = first;
            Operator = op;
            Second = second;
        }
        Operand First { get; set; }
        Token Operator { get; }
        Operand Second { get; set; }
        public override string GetName()
        {
            return "BINARY";
        }

        public override int GetLine() {
            return First.GetLine();
        }

        public override string ToString()
        {
            return string.Format("(expr {1} {0} {2})", First, Operator, Second);
        }

        public override object Eval(Environment environment)
        {
            // TODO UNARY
            var second = Second.Eval(environment);

            var first = First.Eval(environment);

            // TODO Sematically check available operations (string - string impossible)
            // TODO abstract these operations (and maybe variable types)
            switch (Operator.GetName())
            {
                case "PLUS":
                    return ((int)first) + ((int)second);
                case "MINUS":
                    return ((int)first) - ((int)second);
                case "STAR":
                    return ((int)first) * ((int)second);
                case "SLASH":
                    return ((int)first) / ((int)second);
                case "AND":
                    return ((bool)first) && ((bool)second);
                case "EQUAL":
                    return first.Equals(second);
                default:
                    throw new System.NotImplementedException(string.Format("BINARY {0} NOT IMPLEMENTED", Operator.GetName()));
            }
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

    public class Unary : Expression {
        public Unary(Token op, Operand operand) {
            Operand = operand;
            Operator = op;
        }

        public override string GetName() {
            return "UNARY";
        }

        public override int GetLine() {
            return Operand.GetLine();
        }

        public override string ToString()
        {
            return string.Format("(expr {0} {1})", Operator, Operand);
        }

        Operand Operand { get; set; }
        Token Operator { get; set; }

        public override object Eval(Environment environment)
        {
            // TODO UNARY
            var first = Operand.Eval(environment);
            if (Operator == null) {
                return Operand.Eval(environment);
            }

            // TODO Sematically check available operations (string - string impossible)
            // TODO abstract these operations (and maybe variable types)
            switch (Operator.GetName())
            {
                case "PLUS":
                    return (int) first;
                case "MINUS":
                    return -((int) first);
                case "NOT":
                    return !((bool) first);
                default:
                    throw new System.NotImplementedException(string.Format("UNARY {0} NOT IMPLEMENTED", Operator.GetName()));
            }
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
}
