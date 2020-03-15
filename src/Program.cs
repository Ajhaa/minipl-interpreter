using System;
using System.IO;
using System.Collections.Generic;

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
            List<Token> tokens = null;
            try {
                tokens = new Scanner(fileAsString).Tokenize();
            } catch {
                Console.WriteLine("Scanner exited with status code 1");
                return 1;
            }

            // foreach (var token in tokens)
            // {
            //   Console.Write(token + " ");
            // }
            var program = new LLParser(tokens).Parse();
            if (program == null) {
                Console.WriteLine("Parser exited with status code 1");
                return 1;
            }
            var environment = new Analyzer(program).Analyze();
            if (environment == null)
            {
                Console.WriteLine("Analyzer with status code 1");
                return 1;
            }

            // Console.WriteLine();



            try {
                new Interpreter(program, environment).Interpret();
            } catch {
                Console.WriteLine("Interpreter exited with status code 1");
                return 1;
            }
            return 0;
        }
    }
}
