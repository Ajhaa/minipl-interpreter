using System.Collections.Generic;
using System;
class Analyzer {
    public Analyzer(List<Statement> program) {
        this.program = program;
    }
    private List<Statement> program;

    private Environment environment = new Environment();

    public Environment Analyze() {
        var valid = true;
        foreach (var stmt in program) {
            valid = stmt.Analyze(environment) && valid;
        }
        if (!valid) {
            return null;
        }
        return environment;
    }
}