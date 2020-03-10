using System.Collections.Generic;

class Analyzer
{

    public Analyzer(List<Statement> program)
    {
        var environment = new Environment();
        var expressionAnalyzer = new ExpressionAnalyzer(environment);
        statementAnalyzer = new StatementAnalyzer(program, expressionAnalyzer, environment);
    }

    private StatementAnalyzer statementAnalyzer;

    public Environment Analyze()
    {
        return statementAnalyzer.Analyze();
    }
}