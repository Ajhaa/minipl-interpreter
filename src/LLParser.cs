using System.Collections.Generic;
using System;
using static TokenType;
class LLParser
{
    public LLParser(List<Token> tokens)
    {
        this.tokens = tokens;
    }

    private List<Token> tokens;

    private List<Statement> program = new List<Statement>();

    private int index = 0;
    private Token current;

    public List<Statement> Parse()
    {
        while (index < tokens.Count)
        {
            statement();
        }
        return program;
    }

    private Token advance() {
        var current = tokens[index];
        index++;
        return current;
    }

    private Token match(string name) {
        current = advance();
        if (current.GetName() != name) {
            index--;
            throw new Exception(string.Format("Expected {1}, got {0}", current.GetName(), name));
        }

        return current;
    }

    private Token lookahead() 
    {
        if (index + 1< tokens.Count) {
            return tokens[index+1];
        }
        return null;
    } 

    private void statement() {
        current = advance();
        switch (current.GetName()) {
            case "var":
                varStatement();
                break;
            case "IDENTIFIER":
                assignStatement();
                break;
            case "for":
                forStatement();
                break;
            case "read":
                readStatement();
                break;
            case "print":
                printStatement();
                break;
            case "assert":
                assertStatement();
                break;
            default:
                throw new Exception(string.Format("Unexpected symbol {0}", current));
        }
        match("SEMICOLON");
    }


    private void varStatement() {
        var ident = new VarIdentifier(match("IDENTIFIER").Value);
        match("COLON");
        var type = advance();
        try {
            match("ASSIGN");
            program.Add(new Statement.Declarement(ident, type.Value, expression()));
        } catch {
            program.Add(new Statement.Declarement(ident, type.Value, null));
        }
    }

    private void assignStatement() {
        var ident = new VarIdentifier(current.Value);
        match("ASSIGN");
        program.Add(new Statement.Assignment(ident, expression()));
    }

    private void forStatement() {

    }

    private void readStatement() {
        var ident = new VarIdentifier(match("IDENTIFIER").Value);
        program.Add(new Statement.Read(ident));
    }

    private void printStatement() {
        program.Add(new Statement.Print(expression()));
    }

    private void assertStatement() {

    }

    private Expression expression() {
        if (isOperator(lookahead())) {
            return binaryExpression();
        }
        
        return unaryExpression();
    }

    private Expression binaryExpression() {
        var left = operand();
        var operation = advance();
        var right = operand();

        return new Expression.Binary(left, operation, right);
    }

    private Expression unaryExpression() {
        var next = lookahead();
        if (isOperator(next)) {
            return new Expression.Unary(advance(), operand());
        }
        return new Expression.Unary(null, operand());
    }

    private Operand operand() {
        current = advance();
        if (current.GetName() == "LEFT_PAREN") {
            var opr = new Operand(expression());
            match("RIGHT_PAREN");
            return opr;
        }

        if (current.GetName() == "IDENTIFIER") {
            return new Operand(new VarIdentifier(current.Value));
        }

        return new Operand(current);
    }
    
    private bool isOperator(Token t)
    {
        var type = t.Type;
        return type == PLUS || type == MINUS || type == STAR || type == SLASH || type == AND || type == NOT || type == EQUAL;
    }
}