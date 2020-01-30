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
      check(tokens[index]);
    }
    return program;
  }

  private void check(Symbol next) {
    switch (next.GetName()) {
      case "OPERAND":
        if (stack.Count > 0) {
          expr(next);
          index++;
          return;
        }

        shift(next);
        return;

      case "INTEGER":
        check(new Operand(next));
        return;

      case "STRING":
        check(new Operand(next));
        return;

      default:
        shift(next);
        return;
    }
  }

  private void shift(Symbol next) {
    stack.Push(next);
    index++;
  }

  private void reduce() {

  }

  private void expr(Symbol next) {
    var second = (Operand) next;
    var op = (Token) stack.Pop();
    Operand first = null;
    if (stack.Count > 0) {
      first = (Operand) stack.Pop();
    }

    stack.Push(new Expression(first, op, second)); 
    Console.WriteLine("RESULT: " + stack.Peek());
  }
}