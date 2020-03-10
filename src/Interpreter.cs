using System.Collections.Generic;
using System;
class Interpreter : Statement.Visitor<object>
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

    private void runtimeError(Symbol context, string template, params object[] args) {
        ErrorWriter.Write(context, template, args);
        throw new Exception();
    }

    public object visitPrintStmt(Statement.Print stmt)
    {
        Console.Write(stmt.Content.Eval(environment).ToString());
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
        environment.Assign(stmt.Identifier.Name, stmt.Initializer.Eval(environment));
        return null;
    }

    public object visitAssignmentStmt(Statement.Assignment stmt)
    {
        if (!environment.Assign(stmt.Identifier.Name, stmt.Value.Eval(environment)))
        {
            // TODO is this even possible?
            runtimeError(stmt.Identifier, "Cannot assign to uninitialized variable {0}", stmt.Identifier.Name);
        }
        return null;
    }

    public object visitForStmt(Statement.For stmt)
    {
        var counter = (int)stmt.RangeStart.Eval(environment);
        var end = (int)stmt.RangeEnd.Eval(environment);
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
        if (!((bool) stmt.Expression.Eval(environment))) {
            Console.WriteLine(string.Format("Line {0}: Assertion failed", stmt.GetLine()));
        }

        return null;
    }
}