using System.Collections.Generic;
using System;
using static TokenType;
class Parser {
  public Parser(List<Token> tokens) {
    this.tokens = tokens;
  }

  private List<Token> tokens;
  private Stack<Symbol> stack = new Stack<Symbol>();
  private List<Statement> program = new List<Statement>();

  private int index = 0;


  public List<Statement> Parse() {
    while (index < tokens.Count) {
      foreach (Symbol s in stack) {
        Console.Write(s + " ");
      }
      Console.Write("\n");
     // Console.WriteLine("Current: " + tokens[index]);
      stackAdd(tokens[index]);
    }
    return program;
  }


  private void stackAdd(Symbol s) {
    switch (s.GetName()) {
      case "SEMICOLON":
        statement();
        return;
      case "OPERAND":
        if (stack.Count > 0) {
          expr(s);
          return;
        }

        shift(s);
        return;

      case "IDENTIFIER":
        stackAdd(new VarIdentifier(((Token) s).Value));
        return;

      case "INTEGER":
        stackAdd(new Operand(s));
        return;

      case "STRING":
        stackAdd(new Operand(s));
        return;

      case "RIGHT_PAREN":
        parens();
        return;
        
      default:
        shift(s);
        return;
    }
  }

  private void statement() {
    var next = stack.Pop();

    switch (next.GetName()) {
      // TODO better way to handle lone int literals?
      case "OPERAND":
        stack.Push(new Expression(null, null, (Operand) next));
        statement();
        return;
      case "EXPRESSION":
        if (stack.Peek().GetName() == "ASSIGN") {
          assignment(next);
        } else {
          //printStatement();
          throw new System.NotImplementedException("No print yet, sorry!");
        }
        break;
      default:
        throw new System.NotImplementedException("Statements missing");
    }
    program.Add((Statement)stack.Pop());
  }

  private void assignment(Symbol s) {
    stack.Pop();
    var next = stack.Peek();
    if (next.GetName() == "KEYWORD") {
      declaration(s);
      return;
    }

    var identifier = (VarIdentifier) stack.Pop();
    stackAdd(new Statement.Assignment(identifier, (Expression) s));
  }

  private void declaration(Symbol s) {
    var type = (Token) stack.Pop();
    // TODO check that this is colon
    stack.Pop();
    var identifier = (VarIdentifier) stack.Pop();
    try {
      stack.Pop();
    } catch (Exception e) {
      throw new System.Exception("Expected 'var' for variable declaration");
    }
    stackAdd(new Statement.Declarement(identifier, type.Value, (Expression) s));
  }

    private void shift(Symbol next) {
    stack.Push(next);
    index++;
  }

  private void expr(Symbol next) {
    var second = (Operand) next;
    var op = (Token) stack.Pop();
    if (!isOperator(op)) {
      stack.Push(op);
      shift(next);
      return;
    }
    Operand first = null;
    if (stack.Count > 0) {
      first = (Operand) stack.Pop();
    }

    stackAdd(new Expression(first, op, second)); 
  }

  private void parens() {
    var expression = (Expression) stack.Pop();
    Console.WriteLine(expression);
    var matchingParen = (Token) stack.Pop();
    if (matchingParen.Type != LEFT_PAREN) {
      throw new Exception("Unmatched right paren");
    }

    stackAdd(new Operand(expression));
  }

  private bool isOperator(Token t) {
    var type = t.Type;
    return type == PLUS || type == MINUS || type == STAR || type == SLASH;
  }
}