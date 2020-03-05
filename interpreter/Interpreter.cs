using System.Collections.Generic;

class Interpreter {

    public Interpreter(List<Statement> program, Environment environment) {
        this.program = program;
        this.environment = environment;
    }
    private List<Statement> program;

    private Environment environment;

    public void Interpret() {
        foreach (var stmt in program) {
            stmt.Interpret(environment);
        }
    } 

}