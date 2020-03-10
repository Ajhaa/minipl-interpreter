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
    }
}
