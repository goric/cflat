using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTBase: ASTExpression
    {
        public Descriptor Descriptor { get; set; }

        public ASTBase ()
        {
        }

        public override String Print (int depth)
        {
            return "base";
        }
        
        public override void Visit (Visitor v)
        {
            v.VisitBase(this);
        }
    }
}
