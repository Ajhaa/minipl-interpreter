using System.Collections.Generic;

abstract class Statement : Symbol
{
    public abstract string GetName();
    public abstract int GetLine();
    public abstract T Accept<T>(Visitor<T> visitor);

    public interface Visitor<T>
    {
        T visitPrintStmt(Print stmt);
        T visitReadStmt(Read stmt);
        T visitDeclarementStmt(Declarement stmt);
        T visitAssignmentStmt(Assignment stmt);
        T visitForStmt(For stmt);
        T visitAssertStmt(Assert stmt);
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

        public override int GetLine() {
            return Content.GetLine();
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

        public override int GetLine() {
            return Target.GetLine();
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

        public override int GetLine() {
            return Identifier.GetLine();
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

        public override int GetLine() {
            return Identifier.GetLine();
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

    public class For : Statement
    {
        public For(VarIdentifier identifier, Expression start, Expression end, List<Statement> block)
        {
            Identifier = identifier;
            RangeStart = start;
            RangeEnd = end;
            Block = block;
        }

        public override string GetName()
        {
            return "FOR";
        }

        public override int GetLine() {
            return Identifier.GetLine();
        }

        public VarIdentifier Identifier { get; }
        public Expression RangeStart { get; }
        public Expression RangeEnd { get; }
        public List<Statement> Block { get; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitForStmt(this);
        }
    }

    public class Assert : Statement
    {
        public Assert(Expression expr)
        {
            Expression = expr;
        }

        public override string GetName()
        {
            return "ASSERT";
        }

        public override int GetLine() {
            return Expression.GetLine();
        }
        public Expression Expression { get; }

        public override T Accept<T>(Visitor<T> visitor)
        {
            return visitor.visitAssertStmt(this);
        }
    }
}