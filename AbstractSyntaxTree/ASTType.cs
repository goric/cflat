using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTType : ASTNode
    {
        public CFlatType Type { get; set; }

        public override void Visit (Visitor v)
        {
            v.VisitType(this);
        }
    }
}
