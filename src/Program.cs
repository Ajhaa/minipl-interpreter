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
            var program = new LLParser(tokens).Parse();
            Console.WriteLine("PROGRAM:");
            foreach (var stmt in program) {
              Console.WriteLine(stmt);
            }
            var environment = new Analyzer(program).Analyze();
            if (environment == null)
            {
                Console.WriteLine("Exited with status code 1");
                return 1;
            }

            // Console.WriteLine();




            new Interpreter(program, environment).Interpret();
            return 0;
        }
    }
}
