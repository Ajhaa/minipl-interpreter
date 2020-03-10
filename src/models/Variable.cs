class Variable {
    public Variable(string type, object value) {
        Type = type;
        Value = value;
        Locked = false;
    }



    public string Type { get; }
    public object Value { get; set; }
    public bool Locked { get; set; }
}