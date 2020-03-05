using System;

abstract class Statement : Symbol {
  public abstract string GetName();

  public abstract T Accept<T>(Visitor<T> visitor);

  public abstract bool Analyze(Environment environment);
  
  public interface Visitor<T> {
    T visitPrintStmt(Print stmt);
    T visitReadStmt(Read stmt);
    T visitDeclarementStmt(Declarement stmt);

    T visitAssignmentStmt(Assignment stmt);
  }
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

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitPrintStmt(this);
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

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitReadStmt(this);
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

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitDeclarementStmt(this);
    }

    public override string ToString() {
      return string.Format("(declare {0} {1} {2})", Identifier, Type, Initializer);
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

    public override T Accept<T>(Visitor<T> visitor) {
      return visitor.visitAssignmentStmt(this);
    }
    
    public override string ToString() {
      return string.Format("(assign {0} {1})", Identifier, Value);
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