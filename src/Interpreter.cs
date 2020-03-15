using System.Collections.Generic;
using System;
class Interpreter : Statement.Visitor<object>, Expression.Visitor<object>
{

    public Interpreter(List<Statement> program, Environment environment)
    {
        this.program = program;
        this.environment = environment;
    }
    private List<Statement> program;

    private Environment environment;

    public void Interpret()
    {
        foreach (var stmt in program)
        {
            stmt.Accept(this);
        }
    }

    private void runtimeError(Symbol context, string template, params object[] args)
    {
        ErrorWriter.Write(context, template, args);
        throw new Exception();
    }

    public object visitPrintStmt(Statement.Print stmt)
    {
        Console.Write(stmt.Content.Accept(this).ToString());
        return null;
    }

    public object visitReadStmt(Statement.Read stmt)
    {
        var target = stmt.Target;
        var input = Console.ReadLine();

        var type = environment.GetType(target.Name);
        switch (type)
        {
            case "int":
                try
                {
                    environment.Assign(target.Name, Int32.Parse(input));
                }
                catch
                {
                    runtimeError(target, "Could not parse to int");
                }
                break;
            case "bool":
                try
                {
                    environment.Assign(target.Name, Boolean.Parse(input));
                }
                catch
                {
                    runtimeError(target, "Could not parse to bool");
                }
                break;
            default:
                environment.Assign(target.Name, input);
                break;
        }

        return null;
    }

    public object visitDeclarementStmt(Statement.Declarement stmt)
    {
        if (stmt.Initializer == null)
        {
            return null;
        }
        environment.Assign(stmt.Identifier.Name, stmt.Initializer.Accept(this));
        return null;
    }

    public object visitAssignmentStmt(Statement.Assignment stmt)
    {
        if (!environment.Assign(stmt.Identifier.Name, stmt.Value.Accept(this)))
        {
            // TODO is this even possible?
            runtimeError(stmt.Identifier, "Cannot assign to uninitialized variable {0}", stmt.Identifier.Name);
        }
        return null;
    }

    public object visitForStmt(Statement.For stmt)
    {
        var counter = (int)stmt.RangeStart.Accept(this);
        var end = (int)stmt.RangeEnd.Accept(this);
        while (counter <= end)
        {
            environment.Assign(stmt.Identifier.Name, counter);
            foreach (var statement in stmt.Block)
            {
                statement.Accept(this);
            }
            counter++;
        }
        return null;
    }

    public object visitAssertStmt(Statement.Assert stmt)
    {
        if (!((bool)stmt.Expression.Accept(this)))
        {
            Console.WriteLine(string.Format("Line {0}: Assertion failed", stmt.GetLine()));
        }

        return null;
    }

    public object visitBinaryExpr(Expression.Binary expr)
    {
        var second = expr.Second.Accept(this);

        var first = expr.First.Accept(this);

        // TODO Sematically check available operations (string - string impossible)
        // TODO abstract these operations (and maybe variable types)
        switch (expr.Operator.GetName())
        {
            case "PLUS":
                if (first is string) {
                    return first.ToString() + second.ToString();
                }
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
                throw new System.NotImplementedException(string.Format("BINARY {0} NOT IMPLEMENTED", expr.Operator.GetName()));
        }
    }

    public object visitUnaryExpr(Expression.Unary expr)
    {
        var first = expr.Operand.Accept(this);
        if (expr.Operator == null)
        {
            return first;
        }

        // TODO Sematically check available operations (string - string impossible)
        // TODO abstract these operations (and maybe variable types)
        switch (expr.Operator.GetName())
        {
            case "PLUS":
                return (int)first;
            case "MINUS":
                return -((int)first);
            case "NOT":
                return !((bool)first);
            default:
                throw new System.NotImplementedException(string.Format("UNARY {0} NOT IMPLEMENTED", expr.Operator.GetName()));
        }
    }

    public object visitOperandExpr(Expression.Operand expr)
    {
        var value = expr.Value;
        switch (value.GetName())
        {
            case "UNARY":
            case "BINARY":
                return ((Expression)value).Accept(this);
            case "INTEGER":
                return Int32.Parse(((Token)value).Value);
            case "VAR_IDENTIFIER":
                var ident = (VarIdentifier)value;
                return environment.Get(ident.Name);
            default:
                return ((Token)value).Value;
        }
    }
}