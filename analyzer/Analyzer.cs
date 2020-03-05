using System.Collections.Generic;
using System;
class Analyzer {
    public Analyzer(List<Statement> program) {
        this.program = program;
    }
    private List<Statement> program;

    private Dictionary<string, Variable> environment = new Dictionary<string, Variable>();

    public Dictionary<string, Variable> Analyze() {
        var valid = true;
        foreach (var stmt in program) {
            valid = stmt.Analyze(environment);
        }
        if (!valid) {
            return null;
        }
        return environment;
    }
}