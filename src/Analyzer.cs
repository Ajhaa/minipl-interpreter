using System.Collections.Generic;
using System;
class Analyzer : Statement.Visitor<bool>
{
    public Analyzer(List<Statement> program)
    {
        this.program = program;
    }
    private List<Statement> program;

    private Environment environment = new Environment();

    public Environment Analyze()
    {
        var valid = true;
        foreach (var stmt in program)
        {
            valid = stmt.Accept(this) && valid;
        }

        if (!valid)
        {
            return null;
        }
        return environment;
    }

    public bool visitPrintStmt(Statement.Print stmt)
    {
        return stmt.Content.Type(environment) != null;
    }

    public bool visitReadStmt(Statement.Read stmt)
    {
        if (environment.Contains(stmt.Target.Name))
        {
            return true;
        }
        else
        {
            Console.WriteLine(string.Format("Cannot read to uninitialized variable {0}", stmt.Target.Name));
            return false;
        }
    }

    public bool visitDeclarementStmt(Statement.Declarement stmt)
    {
        var identifier = stmt.Identifier;
        var initializer = stmt.Initializer;
        var type = stmt.Type;

        var valid = true;
        if (environment.Contains(identifier.Name))
        {
            Console.WriteLine(string.Format("Cannot initialize variable '{0}' twice", identifier.Name));
            valid = false;
        }

        if (initializer == null)
        {
            environment.Declare(identifier.Name, type);
            return valid;
        }

        var initializertype = initializer.Type(environment);
        if (initializertype == null)
        {
            valid = false;
        }
        else if (initializertype != type)
        {
            Console.WriteLine(string.Format("Cannot initialize variable '{0}' of type {1} to type {2}", identifier.Name, type, initializertype));
            valid = false;
        }

        environment.Declare(identifier.Name, type);
        return valid;
    }

    public bool visitAssignmentStmt(Statement.Assignment stmt)
    {
        var identifier = stmt.Identifier;
        if (!environment.Contains(identifier.Name))
        {
            Console.WriteLine(string.Format("Cannot assign to uninitialized variable '{0}'", identifier.Name));
            return false;
        }

        var variableType = environment.GetType(identifier.Name);

        var initializerType = stmt.Value.Type(environment);
        if (initializerType == null)
        {
            return false;
        }
        if (initializerType != variableType)
        {
            Console.WriteLine(string.Format("Cannot assign value of type {2} to variable {0} of type {1}", identifier.Name, variableType, initializerType));
            return false;
        }

        return true;
    }
}
