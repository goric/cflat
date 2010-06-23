using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;

namespace CFlat.SemanticPasses
{
    public class ThirdPass : SecondPass
    {

        public ThirdPass(ASTNode treeNode)
            : base(treeNode)
        {

        }
    }
}
