// using System.Collections.Generic;
// using System;
// using static TokenType;
// class Parser
// {
//     public Parser(List<Token> tokens)
//     {
//         this.tokens = tokens;
//     }

//     private List<Token> tokens;
//     private Stack<Symbol> stack = new Stack<Symbol>();
//     private List<Statement> program = new List<Statement>();

//     private int index = 0;

//     public List<Statement> Parse()
//     {
//         while (index < tokens.Count)
//         {
//             // foreach (Symbol s in stack) {
//             //   Console.Write(s + " ");
//             // }
//             // Console.Write("\n");
//             stackAdd(tokens[index]);
//         }
//         return program;
//     }


//     private void stackAdd(Symbol s)
//     {
//         switch (s.GetName())
//         {
//             case "SEMICOLON":
//                 statement();
//                 return;
//             case ""

//             case "OPERAND":
//                 expr(s);
//                 break;
//             case "VAR_IDENTIFIER":
//                 if (rawVarIdent()) {
//                     shift(s);
//                     return;
//                 }

//                 stackAdd(new Operand(s));
//                 return;
//             case "IDENTIFIER":
//                 stackAdd(new VarIdentifier(((Token)s).Value));
//                 return;

//             case "INTEGER":
//             case "STRING":
//                 stackAdd(new Operand(s));
//                 return;

//             case "RIGHT_PAREN":
//                 parens();
//                 return;

//             default:
//                 shift(s);
//                 return;
//         }
//     }

//     private void statement()
//     {
//         var next = stack.Pop();

//         switch (next.GetName())
//         {
//             // TODO better way to handle lone int literals and identifiers?
//             case "VAR_IDENTIFIER":
//                 // TODO what if other keyword?
//                 var peek = stack.Peek();
//                 if (peek.GetName() == "KEYWORD")
//                 {
//                     var kw = (Token)peek;
//                     if (kw.Value == "read")
//                     {
//                         readStatement(next);
//                         break;
//                     }
//                 }
//                 stack.Push(new Expression(null, null, new Operand(next)));
//                 return;

//             case "OPERAND":
//                 stack.Push(new Expression(null, null, (Operand)next));
//                 statement();
//                 return;
//             case "KEYWORD":
//                 var keyword = (Token)next;
//                 switch (keyword.Value)
//                 {
//                     case "int":
//                     case "string":
//                     case "bool":
//                         stack.Push(keyword);
//                         declaration(null);
//                         break;
//                     default:
//                         Console.WriteLine(string.Format("Unexpected keyword: {0}", keyword));
//                         //TODO ERROR HANDLING
//                         return;
//                 }
//                 break;
//             case "EXPRESSION":
//                 if (stack.Peek().GetName() == "ASSIGN")
//                 {
//                     assignment(next);
//                 }
//                 else
//                 {
//                     printStatement(next);
//                 }
//                 break;
//             default:
//                 throw new System.NotImplementedException("Statements missing");
//         }
//         program.Add((Statement)stack.Pop());
//     }

//     private void assignment(Symbol s)
//     {
//         stack.Pop();
//         var next = stack.Peek();
//         if (next.GetName() == "KEYWORD")
//         {
//             declaration(s);
//             return;
//         }

//         var identifier = (VarIdentifier)stack.Pop();
//         stackAdd(new Statement.Assignment(identifier, (Expression)s));
//     }

//     private void declaration(Symbol s)
//     {
//         var type = (Token)stack.Pop();
//         // TODO check that this is colon
//         stack.Pop();
//         var identifier = (VarIdentifier)stack.Pop();
//         try
//         {
//             stack.Pop();
//         }
//         catch (Exception e)
//         {
//             throw new System.Exception("Expected 'var' for variable declaration");
//         }
//         stackAdd(new Statement.Declarement(identifier, type.Value, (Expression)s));
//     }

//     private void shift(Symbol next)
//     {
//         stack.Push(next);
//         index++;
//     }

//     private bool check(string type)
//     {
//         return stack.Peek().GetName() == type;
//     }

//     private Symbol lookahead() 
//     {
//         if (index + 1< tokens.Count) {
//             return tokens[index+1];
//         }
//         return null;
//     }

//     private void printStatement(Symbol next)
//     {
//         stack.Pop();
//         stackAdd(new Statement.Print((Expression)next));
//     }

//     private void readStatement(Symbol next)
//     {
//         stack.Pop();
//         stackAdd(new Statement.Read((VarIdentifier)next));
//     }

//     private void expr(Symbol next)
//     {
//         var op = stack.Pop();

//         Operand first = null;
//         if (stack.Count > 0)
//         {
//             if (stack.Peek().GetName() == "VAR_IDENTIFIER")
//             {
//                 first = new Operand(stack.Pop());
//             }
//             else if (stack.Peek().GetName() == "OPERAND")
//             {
//                 first = (Operand)stack.Pop();
//             }
//             stackAdd(new Expression.Binary(first, (Token)op, (Operand)next));
//         }

//         if (next.GetName() == "VAR_IDENTIFIER")
//         {
//             next = new Operand(next);
//         }

//         stackAdd(new Expression.Unary((Token)op, (Operand)next));
//     }

//     private void parens()
//     {
//         var expression = (Expression)stack.Pop();
//         var matchingParen = (Token)stack.Pop();
//         if (matchingParen.Type != LEFT_PAREN)
//         {
//             throw new Exception("Unmatched right paren");
//         }

//         stackAdd(new Operand(expression));
//     }

//     private bool isOperator(Symbol t)
//     {
//         try
//         {
//             var token = (Token)t;
//             var type = token.Type;
//             return type == PLUS || type == MINUS || type == STAR || type == SLASH || type == AND || type == NOT || type == EQUAL;
//         }
//         catch
//         {
//             return false;
//         }
//     }

//     private bool rawVarIdent() {
//         return stack.Count == 0 || stack.Peek().GetName() == "KEYWORD";
//     }
// }