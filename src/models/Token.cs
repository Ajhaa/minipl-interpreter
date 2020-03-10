enum TokenType
{
    SEMICOLON, COLON, ASSIGN, RANGE, KEYWORD, IDENTIFIER, STRING, INTEGER,
    PLUS, MINUS, STAR, SLASH, LESS_THAN, GREATER_THAN, EQUAL, AND, NOT,
    RIGHT_PAREN, LEFT_PAREN
}

class Token : Symbol
{

    public Token(TokenType type, string value, int line)
    {
        Type = type;
        Value = value;
        this.line = line;
    }

    public Token(TokenType type, int line) : this(type, "", line) { }

    public TokenType Type { get; }
    public string Value { get; }
    private int line;
    public string GetName()
    {
        if (Type == TokenType.KEYWORD) {
            return Value;
        }
        return Type.ToString();
    }

    public int GetLine() {
        return line;
    }

    public override string ToString()
    {
        if (Value == "") return string.Format("<{0}>", Type);
        return string.Format("<{0}>", string.Join(" ", Type, Value));
    }
}