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

    public object visitPrintStmt(Statement.Print stmt)
    {
        Console.WriteLine(stmt.Content.Eval(environment).ToString());
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
                    Console.WriteLine("Could not parse to int");
                }
                break;
            case "bool":
                try
                {
                    environment.Assign(target.Name, Boolean.Parse(input));
                }
                catch
                {
                    Console.WriteLine("Could not parse to bool");
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
            Console.WriteLine(string.Format("Cannot assign to uninitialized variable {0}", stmt.Identifier.Name));
        }
        return null;
    }

}