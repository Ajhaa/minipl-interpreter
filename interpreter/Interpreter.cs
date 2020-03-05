using System.Collections.Generic;

class Interpreter {

    public Interpreter(List<Statement> program) {
        this.program = program;
    }
    private List<Statement> program;

    private Dictionary<string, object> environment = new Dictionary<string, object>();

    public void Interpret() {
        foreach (var stmt in program) {
            stmt.Interpret(environment);
        }
    } 

}