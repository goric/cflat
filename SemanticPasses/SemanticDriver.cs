using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;

namespace CFlat.SemanticPasses
{
    public sealed class SemanticDriver
    {
        //returns object for now, not sure where i'm going with this...
        public static object Analyze(ASTNode treeNode)
        {
            //all of the passes need to share a ScopeManager rather than creating a new one in the constructor
            ScopeManager scopeMgr = new ScopeManager();

            //one at a time and bail on failure?
            FirstPass semPass1 = new FirstPass(treeNode, scopeMgr);
            semPass1.Run();
            if(semPass1.Failed) return null;

            SecondPass semPass2 = new SecondPass(treeNode, scopeMgr);
            semPass2.Run();
            if(semPass2.Failed) return null;

            ThirdPass semPass3 = new ThirdPass(treeNode, scopeMgr);
            semPass3.Run();
            if(semPass3.Failed) return null;

            //null for now - change later
            return null;
        }
    }
}
