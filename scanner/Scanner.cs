using System.Collections.Generic;
using static TokenType;
class Scanner
{
  public Scanner(string input)
  {
    this.input = input;
  }

  private string input;

  // private int start = 0;
  private int index = 0;
  private int line = 1;
  private List<Token> tokens = new List<Token>();

  public List<Token> Tokenize()
  {
    while (index < input.Length)
    {
      consumeToken();
      index++;
    }
    return tokens;
  }

  private void addToken(Token t)
  {
    tokens.Add(t);
  }

  private void consumeToken()
  {
    char current = input[index];
    switch (current)
    {
      // unambiguous single char tokens
      case ';':
        addToken(new Token(SEMICOLON));
        return;
      case '+':
        addToken(new Token(PLUS));
        return;
      case '-':
        addToken(new Token(MINUS));
        return;
      case '*':
        addToken(new Token(STAR));
        return;
      case '/':
        addToken(new Token(SLASH));
        return;
      case '<':
        addToken(new Token(LESS_THAN));
        return;
      case '>':
        addToken(new Token(GREATER_THAN));
        return;
      case '=':
        addToken(new Token(EQUAL));
        return;
      case '&':
        addToken(new Token(AND));
        return;
      case '!':
        addToken(new Token(NOT));
        return;
      case ')':
        addToken(new Token(RIGHT_PAREN));
        return;
      case '(':
        addToken(new Token(LEFT_PAREN));
        return;
      // colon or assignment
      case ':':
        if (lookahead() == '=')
        {
          index++;
          addToken(new Token(ASSIGN));
          return;
        }
        addToken(new Token(COLON));
        return;
      // range
      case '.':
        if (lookahead() == '.')
        {
          index++;
          addToken(new Token(RANGE));
          return;
        }
        else
        {
          throw new System.Exception("PANIC");
        }
      case '"':
        makeString();
        return;
      case ' ':
        return;
      case '\n':
        line++;
        return;
      default:
        if (isLetter(current))
        {
          identOrKeyword();
          return;
        }

        if (isNumber(current))
        {
          makeInteger();
          return;
        }
        throw new System.Exception("PANIC");
    }
  }

  private bool isLetter(char c)
  {
    return (c >= 'a' && c <= 'z') ||
           (c >= 'A' && c <= 'Z');
  }

  private bool isNumber(char c)
  {
    return (c >= '0' && c <= '9');
  }

  private bool isLegalChar(char c)
  {
    return isLetter(c) || isNumber(c) || c == '_';
  }

  private char lookahead()
  {
    if (index + 1 >= input.Length)
    {
      return (char)3;
    }
    return input[index + 1];
  }

  private void identOrKeyword()
  {
    int stringStart = index;
    while (isLegalChar(lookahead()))
    {
      index++;
    }

    var result = input.Substring(stringStart, index - stringStart + 1);
    if (Keywords.isKeyword(result))
    {
      addToken(new Token(KEYWORD, result));
    }
    else
    {
      addToken(new Token(IDENTIFIER, result));
    }

  }

  private void makeString()
  {
    index++;
    int stringStart = index;
    while (input[index] != '"')
    {
      if (index >= input.Length)
      {
        throw new System.Exception("UNTERMINATED STRING");
      }
      index++;
    }

    addToken(new Token(STRING, input.Substring(stringStart, index - stringStart)));
  }

  // TODO reject if chars before or after number without space 
  // TODO handle sign
  private void makeInteger()
  {
    int numberStart = index;

    while (true)
    {
      var next = lookahead();
      if (isLetter(next))
      {
        throw new System.Exception("Unexpected char while parsing integer literal");
      }

      if (!isNumber(next))
      {
        break;
      }
      index++;
    }

    addToken(new Token(INTEGER, input.Substring(numberStart, index - numberStart + 1)));
  }
}