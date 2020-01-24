enum TokenType
{
  SEMICOLON, COLON, ASSIGN, RANGE, KEYWORD, IDENTIFIER, STRING, INTEGER,
  PLUS, MINUS, STAR, SLASH, LESS_THAN, GREATER_THAN, EQUAL, AND, NOT,
  RIGHT_PAREN, LEFT_PAREN
}

class Token
{

  public Token(TokenType type, string value)
  {
    Type = type;
    Value = value;
  }

  public Token(TokenType type) : this(type, "") { }

  public TokenType Type { get; }
  public string Value { get; }

  public override string ToString()
  {
    return string.Format("<{0}, {1}>", Type, Value);
  }
}