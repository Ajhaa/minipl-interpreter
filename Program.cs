using System;
using System.IO;

namespace minipl_interpreter
{
  class Program
  {
    static int Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine("Please enter a minipl program path as argument");
        return 1;
      }
      var fileAsString = File.ReadAllText(args[0]);
      var tokens = new Scanner(fileAsString).Tokenize();

      // foreach (var token in tokens)
      // {
      //   Console.Write(token + " ");
      // }
      var program = new Parser(tokens).Parse();
      Console.WriteLine();

      new Interpreter(program).Interpret();

      Console.Write("\n");
      return 0;
    }
  }
}
