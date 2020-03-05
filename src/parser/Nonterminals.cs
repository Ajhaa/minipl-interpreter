using System.Collections.Generic;
using System;
class VarIdentifier : Symbol {
  public VarIdentifier(string name) {
    Name = name;
  }

  public string Name { get; }
  public string GetName() {
    return "VAR_IDENTIFIER";
  }

  public override string ToString() {
    return string.Format("(ident {0})", Name);
  }
}

class Expression : Symbol {
  public Expression(Operand first, Token op, Operand second) {
    First = first;
    Operator = op;
    Second = second;
  } 
  Operand First { get; }
  Token Operator { get; }
  Operand Second { get; }
  public string GetName() {
    return "EXPRESSION";
  }

  public override string ToString() {
    return string.Format("(expr {1} {0} {2})", First, Operator, Second);
  }

  public object Eval(Environment environment) {
    // TODO UNARY
    var second = Second.Eval(environment);

    if (Operator == null) {
      return second;
    }

    if (First == null) {
      switch (Operator.GetName()) {
        case "NOT":
          return !((bool) second);
        case "MINUS":
          return -((int) second);
        throw new System.NotImplementedException(string.Format("UNARY {0} NOT IMPLEMENTED", Operator.GetName()));
      }
    }

    var first = First.Eval(environment);

    // TODO Sematically check available operations (string - string impossible)
    // TODO abstract these operations (and maybe variable types)
    switch (Operator.GetName()) {
      case "PLUS":
        return ((int) first) + ((int) second);
      case "MINUS":
        return ((int) first) - ((int) second);
      case "STAR":
        return ((int) first) * ((int) second);
      case "SLASH":
        return ((int) first) / ((int) second);
      case "AND":
        return ((bool) first) && ((bool) second);
      case "EQUAL":
        return first.Equals(second);  
      default:
        throw new System.NotImplementedException(string.Format("BINARY {0} NOT IMPLEMENTED", Operator.GetName()));
    }
  }

  public string Type(Environment environment) {
    
    if (First == null) {
      return Second.Type(environment);
    }

    

    var firstType = First.Type(environment);
    var secondType = Second.Type(environment);

    if (firstType == null || secondType == null) {
      return null;
    }
    
    if (firstType != secondType) {
      Console.WriteLine(string.Format("Cannot operate with {0} and {1}", firstType, secondType));
      return null;
    }

    // TODO use eval to get types?
    if (Operator != null &&
          (Operator.Type == TokenType.EQUAL ||
           Operator.Type == TokenType.AND ||
           Operator.Type == TokenType.NOT)) {
        
      return "bool";
    }

    return First.Type(environment);
  }
}

class Operand : Symbol {
  public Operand(Symbol value) {
    Value = value;
  }

  public Symbol Value { get; }

  public string GetName() {
    return "OPERAND";
  }

  public override string ToString() {
    return string.Format("(opnd {0})", Value.ToString());
  }

  public object Eval(Environment environment) {
    switch (Value.GetName()) {
      case "EXPRESSION":
        return ((Expression) Value).Eval(environment);
      case "INTEGER":
        return Int32.Parse(((Token) Value).Value);
      case "VAR_IDENTIFIER":
        var ident = (VarIdentifier) Value; 
        return environment.Get(ident.Name);
      default:
        return ((Token) Value).Value;
    }
  }

  public string Type(Environment environment) {
    switch (Value.GetName()) {
      case "EXPRESSION":
        return ((Expression) Value).Type(environment);
      case "INTEGER":
        return "int";
      case "STRING":
        return "string";
      case "VAR_IDENTIFIER":
        var ident = (VarIdentifier) Value; 

        if (!environment.Contains(ident.Name)) {
          Console.WriteLine(string.Format("Cannot operate on uninitalized variable '{0}'", ident.Name));
          return null;
        }

        return environment.GetType(ident.Name);
      default:
        return null;
    }
  }
}