using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTSelf : ASTExpression
    {
        public Descriptor Descriptor { get; set; }
        
        public ASTSelf ()
        {
        }

        public override String Print (int depth)
        {
            return "self";
        }
        
        public override void Visit (Visitor v)
        {
            v.VisitSelf(this);
        }
    }
}
