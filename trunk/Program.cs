using System;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;

using LexicalAnalysis;
using SyntaxAnalysis;
using AbstractSyntaxTree;
using ILCodeGen;


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

            if (SemanticPasses.SemanticDriver.Analyze(root))
            {
                //I fail at string processing but w/e
                
                CodeGenerator cg = new CodeGenerator(sourceFile.Substring(sourceFile.LastIndexOf("\\") + 1).Replace(".cf", ""));

                cg.Generate(root);

                cg.WriteAssembly();
            }

#if DEBUG
            Console.Write("Press any key to exit...");
            Console.ReadKey();
#endif
        }

        private static void PrintUsage ()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("  CFlat.exe sourceFile");
        }
    }
}
