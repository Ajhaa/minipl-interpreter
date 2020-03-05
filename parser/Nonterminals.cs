using System.Collections.Generic;
using System;

abstract class Statement : Symbol {
  public abstract string GetName();
  public abstract void Interpret(Environment environment);
  public abstract bool Analyze(Environment environment);

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

    public override void Interpret(Environment environment) {
      Console.WriteLine(Content.Eval(environment).ToString());
    }

    public override bool Analyze(Environment environment) {
      return Content.Type(environment) != null;
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

    public override void Interpret(Environment environment) {
      var input = Console.ReadLine();

      var type = environment.GetType(Target.Name);
      switch (type) {
        case "int":
          environment.Assign(Target.Name, Int32.Parse(input));
          break;
        case "bool":
          environment.Assign(Target.Name, Boolean.Parse(input));
          break;
        default:
          environment.Assign(Target.Name, input);
          break;
      }    
    }

    public override bool Analyze(Environment environment) {
      if (environment.Contains(Target.Name)) {
        return true;
      } else {
        Console.WriteLine(string.Format("Cannot read to uninitialized variable {0}", Target.Name));
        return false;
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

    public override void Interpret(Environment environment) {
      if (Initializer == null) {
        return;
      }
      environment.Assign(Identifier.Name, Initializer.Eval(environment));
    }

    public override bool Analyze(Environment environment) {
      var valid = true;
      if (environment.Contains(Identifier.Name)) {
        Console.WriteLine(string.Format("Cannot initialize variable '{0}' twice", Identifier.Name));
        valid = false;
      }
      
      if (Initializer == null) {
        environment.Declare(Identifier.Name, Type);
        return valid;
      }

      var initializerType = Initializer.Type(environment);
      if (initializerType == null) {
        valid = false;
      } else if (initializerType != Type) {
        Console.WriteLine(string.Format("Cannot initialize variable '{0}' of type {1} to type {2}", Identifier.Name, Type, initializerType));
        valid = false;
      }

      environment.Declare(Identifier.Name, Type);
      return valid;    
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

    public override void Interpret(Environment environment) {
      if (!environment.Assign(Identifier.Name, Value.Eval(environment))) {
        Console.WriteLine(string.Format("Cannot assign to uninitialized variable {0}", Identifier.Name));
      }
    }
    public override bool Analyze(Environment environment) {
      if (!environment.Contains(Identifier.Name)) {
        Console.WriteLine(string.Format("Cannot assign to uninitialized variable '{0}'", Identifier.Name));
        return false;
      }

      var variableType = environment.GetType(Identifier.Name);

      var initializerType = Value.Type(environment);
      if (initializerType == null) {
        return false;
      }
      if (initializerType != variableType) {
        Console.WriteLine(string.Format("Cannot assign value of type {2} to variable {0} of type {1}", Identifier.Name, variableType, initializerType));
        return false;
      }

      return true;  
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

  public object Eval(Environment environment) {
    // TODO UNARY
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