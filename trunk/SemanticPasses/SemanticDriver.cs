using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;

namespace CFlat.SemanticPasses
{
    public sealed class SemanticDriver
    {
        /// <summary>
        /// update WJS - changed this to return a bool and stop once the first error has been detected.
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        public static bool Analyze(ASTNode treeNode)
        {
            //all of the passes need to share a ScopeManager rather than creating a new one in the constructor
            ScopeManager scopeMgr = new ScopeManager();

            //one at a time and bail on failure
            return TryRunPass(new FirstPass(treeNode, scopeMgr)) &&
                   TryRunPass(new SecondPass(treeNode, scopeMgr)) &&
                   TryRunPass(new ThirdPass(treeNode, scopeMgr));
        }

        private static bool TryRunPass(ICompilerPass pass)
        {
            try
            {
                pass.Run();
            }
            catch (SourceCodeErrorException ex)
            {
                HandleError(ex, pass.PassName());
                return false;
            }

            return true;
        }

        private static void HandleError(SourceCodeErrorException ex, string passName)
        {
            Console.WriteLine("Error in {0}: {1}", passName, ex.Message);
        }
    }
}
