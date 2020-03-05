using System.Collections.Generic;

class Keywords
{
    private static HashSet<string> Words = new HashSet<string>{
    "var", "for", "end", "in", "do", "read",
    "print", "int", "string", "bool", "assert"
  };

    public static bool isKeyword(string s)
    {
        return Words.Contains(s);
    }
}