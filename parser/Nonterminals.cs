using System.Collections.Generic;
using System;

abstract class Statement : Symbol {
  public abstract string GetName();
  public abstract void Interpret(Dictionary<string, object> environment);

  public class Print : Statement {
    public Print(Expression content) {
      Content = content;
    }
    public Expression Content { get; }
    public override string GetName() {
      return "PRINT";
    }

    public override string ToString() {
      return string.Format("(print {0})", Content);
    }

    public override void Interpret(Dictionary<string, object> environment) {
      Console.WriteLine(Content.Eval(environment).ToString());
    }
  }

  public class Read : Statement {
    public Read(VarIdentifier target) {
      Target = target;
    }
    public VarIdentifier Target { get; }
    public override string GetName() {
      return "READ";
    }

    public override string ToString() {
      return string.Format("(read {0})", Target);
    }

    public override void Interpret(Dictionary<string, object> environment) {
      var input = Console.ReadLine();
      // TODO CHECK TYPE!!!!!111!!
      try {
        environment[Target.Name] = Int32.Parse(input);
      } catch {
        environment[Target.Name] = input;
      }
    }
  }

  public class Declarement : Statement {
    public Declarement(VarIdentifier identifier, string type, Expression initializer) {
      Identifier = identifier;
      Type = type;
      Initializer = initializer;
    }
    public VarIdentifier Identifier { get; }
    public string Type { get; }
    public Expression Initializer { get; }

    public override string GetName() {
      return "DECLAREMENT";
    }

    public override string ToString() {
      return string.Format("(declare {0} {1} {2})", Identifier, Type, Initializer);
    }

    public override void Interpret(Dictionary<string, object> environment) {
      if (Initializer == null) {
        environment.Add(Identifier.Name, null);
        return;
      }
      environment.Add(Identifier.Name, Initializer.Eval(environment));
    }
  }

  public class Assignment : Statement {
    public Assignment(VarIdentifier ident, Expression value) {
      Identifier = ident;
      Value = value;
    }

    public VarIdentifier Identifier { get; }
    public Expression Value { get; }

    public override string GetName() { 
      return "ASSIGNMENT";
    }

    public override string ToString() {
      return string.Format("(assign {0} {1})", Identifier, Value);
    }

    public override void Interpret(Dictionary<string, object> environment) {
      if (!environment.ContainsKey(Identifier.Name)) {
        Console.WriteLine(string.Format("Cannot assign to uninitialized variable {0}", Identifier.Name));
        return;
      }

      environment[Identifier.Name] = Value.Eval(environment);
    }
  }
}



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

  public object Eval(Dictionary<string, object> environment) {
    if (Operator == null) {
      return Second.Eval(environment);
    }
    switch (Operator.GetName()) {
      case "PLUS":
        return ((int) First.Eval(environment)) + ((int) Second.Eval(environment));
      case "MINUS":
        return ((int) First.Eval(environment)) - ((int) Second.Eval(environment));
      case "STAR":
        return ((int) First.Eval(environment)) * ((int) Second.Eval(environment));
      case "SLASH":
        return ((int) First.Eval(environment)) / ((int) Second.Eval(environment));  
      default:
        throw new System.NotImplementedException(string.Format("{0} NOT IMPLEMENTED", Operator.GetName()));
    }
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

  public object Eval(Dictionary<string, object> environment) {
    switch (Value.GetName()) {
      case "EXPRESSION":
        return ((Expression) Value).Eval(environment);
      case "INTEGER":
        return Int32.Parse(((Token) Value).Value);
      case "VAR_IDENTIFIER":
        var ident = (VarIdentifier) Value; 
        return environment.GetValueOrDefault(ident.Name, null);
      default:
        return ((Token) Value).Value;
    }
  }
}