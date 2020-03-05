using System;

abstract class Statement : Symbol
{
    public abstract string GetName();

    public abstract T Accept<T>(Visitor<T> visitor);

    public interface Visitor<T>
    {
        T visitPrintStmt(Print stmt);
        T visitReadStmt(Read stmt);
        T visitDeclarementStmt(Declarement stmt);

        T visitAssignmentStmt(Assignment stmt);
    }

    public class Print : Statement
    {
        public Print(Expression content)
        {
            Content = content;
        }
        public Expression Content { get; }
        public override string GetName()
        {
            return "PRINT";
        }

        public override string ToString()
        {
            return string.Format("(print {0})", Content);
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitPrintStmt(this);
        }
    }

    public class Read : Statement
    {
        public Read(VarIdentifier target)
        {
            Target = target;
        }
        public VarIdentifier Target { get; }
        public override string GetName()
        {
            return "READ";
        }

        public override string ToString()
        {
            return string.Format("(read {0})", Target);
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitReadStmt(this);
        }
    }

    public class Declarement : Statement
    {
        public Declarement(VarIdentifier identifier, string type, Expression initializer)
        {
            Identifier = identifier;
            Type = type;
            Initializer = initializer;
        }
        public VarIdentifier Identifier { get; }
        public string Type { get; }
        public Expression Initializer { get; }

        public override string GetName()
        {
            return "DECLAREMENT";
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitDeclarementStmt(this);
        }

        public override string ToString()
        {
            return string.Format("(declare {0} {1} {2})", Identifier, Type, Initializer);
        }
    }

    public class Assignment : Statement
    {
        public Assignment(VarIdentifier ident, Expression value)
        {
            Identifier = ident;
            Value = value;
        }

        public VarIdentifier Identifier { get; }
        public Expression Value { get; }

        public override string GetName()
        {
            return "ASSIGNMENT";
        }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitAssignmentStmt(this);
        }

        public override string ToString()
        {
            return string.Format("(assign {0} {1})", Identifier, Value);
        }
    }
}