class Statement {

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
    return string.Format("{0} {1} {2}", First, Operator, Second);
  }
}

class Operand : Symbol {
  public Operand(Symbol value) {
    Value = value;
  }

  Symbol Value { get; }

  public string GetName() {
    return "OPERAND";
  }
}