using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;

namespace CFlat.SemanticPasses
{
    public class SecondPass : FirstPass
    {
        private ActualBuilder _actuals;
        private FormalBuilder _formals;

        public SecondPass(ASTNode treeNode)
            : base(treeNode)
        {

        }

        
    }
}
