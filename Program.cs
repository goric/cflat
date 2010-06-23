using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;

using LexicalAnalysis;
using SyntaxAnalysis;
using AbstractSyntaxTree;

namespace CFlat
{
    class Program
    {
        static void Main (string[] args)
        {
            if (args.Length != 1)
            {
                PrintUsage();
                return;
            }

            Compile(args[0]);
        }

        private static void Compile (string sourceFile)
        {
            var scan = new Scanner();
            scan.SetSource(new FileStream(sourceFile, FileMode.Open));

            var parser = new Parser(scan);
            if (!parser.Parse())
                return;

            var root = parser.SyntaxTreeRoot;

            // call this to pretty print the AST
            Console.WriteLine(root.Print(0));

            SemanticPasses.SemanticDriver.Analyze(root);
            
        }

        private static void PrintUsage ()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  CFlat.exe sourceFile");
        }
    }
}
