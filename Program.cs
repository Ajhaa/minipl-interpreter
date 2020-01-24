using System;

namespace minipl_interpreter
{
  class Program
  {
    static int Main(string[] args)
    {
      if (args.Length == 0)
      {
        Console.WriteLine("Please enter a minipl program as (string) argument");
        return 1;
      }

      var tokens = new Scanner(args[0]).Tokenize();

      foreach (var token in tokens)
      {
        Console.Write(token + " ");
      }

      Console.Write("\n");

      return 0;
    }
  }
}
