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
      // foreach (Symbol s in stack) {
      //   Console.Write(s + " ");
      // }
      // Console.Write("\n");
     // Console.WriteLine("Current: " + tokens[index]);
      stackAdd(tokens[index]);
    }
    return program;
  }


  private void stackAdd(Symbol s) {
    switch (s.GetName()) {
      case "OPERAND":
        if (stack.Count > 0) {
          expr(s);
          return;
        }

        shift(s);
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
    Console.WriteLine("RESULT: " + stack.Peek());
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