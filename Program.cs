using System;

namespace minipl_interpreter
{
  class Program
  {
    static void Main(string[] args)
    {
      var tokens = new Scanner("var num : int := 2467;").Tokenize();
      foreach (var token in tokens)
      {
        Console.Write(token + " ");
      }

    }
  }
}
