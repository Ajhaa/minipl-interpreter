class ExpressionAnalyzer : Expression.Visitor<string>
{
    public ExpressionAnalyzer(Environment env)
    {
        environment = env;
    }

    private Environment environment;

    public string visitBinaryExpr(Expression.Binary expr)
    {
        var firstType = expr.First.Accept(this);
        var secondType = expr.Second.Accept(this);

        if (firstType == null || secondType == null)
        {
            return null;
        }

        if (firstType != secondType)
        {
            ErrorWriter.Write(expr.First, "Cannot operate with {0} and {1}", firstType, secondType);
            return null;
        }

        // TODO use eval to get types?
        var type = expr.Operator.Type;
        if (
              (type == TokenType.EQUAL ||
               type == TokenType.AND ||
               type == TokenType.NOT))
        {

            return "bool";
        }

        return expr.First.Accept(this);
    }

    public string visitUnaryExpr(Expression.Unary expr)
    {
        var firstType = expr.Operand.Accept(this);

        // TODO use eval to get types?
        if (expr.Operator != null && expr.Operator.Type == TokenType.NOT)
        {

            return "bool";
        }

        return firstType;
    }

    public string visitOperandExpr(Expression.Operand expr)
    {
        switch (expr.Value.GetName())
        {
            case "UNARY":
            case "BINARY":
                return ((Expression)expr.Value).Accept(this);
            case "INTEGER":
                return "int";
            case "STRING":
                return "string";
            case "VAR_IDENTIFIER":
                var ident = (VarIdentifier)expr.Value;

                if (!environment.Contains(ident.Name))
                {
                    ErrorWriter.Write(ident, "Cannot operate on uninitalized variable '{0}'", ident.Name);
                    return null;
                }

                return environment.GetType(ident.Name);
            default:
                return null;
        }
    }
}