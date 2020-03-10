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
            ErrorWriter.Write(stmt.Target, "Cannot read to uninitialized variable {0}", stmt.Target.Name);
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
            ErrorWriter.Write(identifier, "Cannot initialize variable '{0}' twice", identifier.Name);
            valid = false;
        }

        if (initializer == null)
        {
            environment.Declare(identifier.Name, type);
            return valid;
        }

        var initializerType = initializer.Type(environment);
        if (initializerType == null)
        {
            valid = false;
        }
        else if (initializerType != type)
        {
            ErrorWriter.Write(stmt.Initializer, "Cannot initialize variable '{0}' of type {1} to type {2}", identifier.Name, type, initializerType);
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
            ErrorWriter.Write(identifier, "Cannot assign to uninitialized variable '{0}'", identifier.Name);
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
            ErrorWriter.Write(stmt.Value, "Cannot assign value of type {2} to variable {0} of type {1}", identifier.Name, variableType, initializerType);
            return false;
        }
        if (environment.isLocked(identifier.Name)) {
            ErrorWriter.Write(stmt.Value, "Cannot reassign loop variable {0} during loop", identifier.Name);
            return false;
        }

        return true;
    }

    public bool visitForStmt(Statement.For stmt)
    {
        var identifier = stmt.Identifier;
        if (!environment.Contains(identifier.Name))
        {
            ErrorWriter.Write(identifier, "Usage of uninitialized variable '{0}'", identifier.Name);
            return false;
        }

        if (environment.GetType(identifier.Name) != "int")
        {
            ErrorWriter.Write(identifier, "Loop variable '{0}' must be of type 'int'", identifier.Name);
            return false;
        }

        var startType = stmt.RangeStart.Type(environment);
        if (startType != "int")
        {
            ErrorWriter.Write(stmt.RangeStart, "Range start must be a number");
            return false;
        }
        var endType = stmt.RangeEnd.Type(environment);
        if (endType != "int")
        {
            ErrorWriter.Write(stmt.RangeEnd, "Range end must be a number");
            return false;
        }
        var valid = true;

        // lock the variable so it can't be modified during the loop
        environment.setLock(identifier.Name, true);
        foreach (var statement in stmt.Block)
        {
            valid = statement.Accept(this) && valid;
        }
        environment.setLock(identifier.Name, false);

        return valid;
    }

    public bool visitAssertStmt(Statement.Assert stmt)
    {
        if (stmt.Expression.Type(environment) != "bool")
        {
            ErrorWriter.Write(stmt.Expression, "Cannot assert non-boolean expression");
            return false;
        }

        return true;
    }
}
