using System.Collections.Generic;
using System;
class VarIdentifier : Symbol
{
    public VarIdentifier(string name, int line)
    {
        Name = name;
        this.line = line;
    }

    public string Name { get; }
    private int line;
    public string GetName()
    {
        return "VAR_IDENTIFIER";
    }

    public int GetLine() {
        return line;
    }

    public override string ToString()
    {
        return string.Format("(ident {0})", Name);
    }
}