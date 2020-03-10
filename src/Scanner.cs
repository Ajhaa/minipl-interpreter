using System.Collections.Generic;
using static TokenType;
using System;
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
                addToken(new Token(SEMICOLON, line));
                return;
            case '+':
                addToken(new Token(PLUS, line));
                return;
            case '-':
                addToken(new Token(MINUS, line));
                return;
            case '*':
                addToken(new Token(STAR, line));
                return;
            case '<':
                addToken(new Token(LESS_THAN, line));
                return;
            case '>':
                addToken(new Token(GREATER_THAN, line));
                return;
            case '=':
                addToken(new Token(EQUAL, line));
                return;
            case '&':
                addToken(new Token(AND, line));
                return;
            case '!':
                addToken(new Token(NOT, line));
                return;
            case ')':
                addToken(new Token(RIGHT_PAREN, line));
                return;
            case '(':
                addToken(new Token(LEFT_PAREN, line));
                return;
            // comment or slash
            case '/':
                var next = lookahead();
                if (next == '/')
                {
                    lineComment();
                    return;
                }
                if (next == '*')
                {
                    multiLineComment();
                    return;
                }
                addToken(new Token(SLASH, line));
                return;
            // colon or assignment
            case ':':
                if (lookahead() == '=')
                {
                    index++;
                    addToken(new Token(ASSIGN, line));
                    return;
                }
                addToken(new Token(COLON, line));
                return;
            // range
            case '.':
                if (lookahead() == '.')
                {
                    index++;
                    addToken(new Token(RANGE, line));
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
            addToken(new Token(KEYWORD, result, line));
        }
        else
        {
            addToken(new Token(IDENTIFIER, result, line));
        }

    }

    private void makeString()
    {
        index++;
        int stringStart = index;
        string newString = "";
        while (input[index] != '"')
        {
            if (input[index] == '\\' && input[index + 1] == 'n') {
                newString += '\n';
                index += 2;
                continue;
            }
            if (index >= input.Length)
            {
                throw new System.Exception("UNTERMINATED STRING");
            }
            newString += input[index];
            index++;
        }

        addToken(new Token(STRING, newString, line));
    }

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

        addToken(new Token(INTEGER, input.Substring(numberStart, index - numberStart + 1), line));
    }

    private void lineComment()
    {
        do
        {
            index++;
        } while (input[index] != '\n');
    }

    private void multiLineComment()
    {
        throw new System.NotImplementedException();
    }
}