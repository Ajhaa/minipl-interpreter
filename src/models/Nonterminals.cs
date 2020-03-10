using System.Collections.Generic;
using System;
class VarIdentifier : Symbol
{
    public VarIdentifier(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public string GetName()
    {
        return "VAR_IDENTIFIER";
    }

    public override string ToString()
    {
        return string.Format("(ident {0})", Name);
    }
}


class Operand : Symbol
{
    public Operand(Symbol value)
    {
        Value = value;
    }

    public Symbol Value { get; }

    public string GetName()
    {
        return "OPERAND";
    }

    public override string ToString()
    {
        return string.Format("(opnd {0})", Value.ToString());
    }

    public object Eval(Environment environment)
    {
        switch (Value.GetName())
        {
            case "UNARY":
            case "BINARY":
                return ((Expression)Value).Eval(environment);
            case "INTEGER":
                return Int32.Parse(((Token)Value).Value);
            case "VAR_IDENTIFIER":
                var ident = (VarIdentifier)Value;
                return environment.Get(ident.Name);
            default:
                return ((Token)Value).Value;
        }
    }

    public string Type(Environment environment)
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