using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTExpression : ASTNode
    {
        protected CFlatType Type { get; set; }

        public ASTExpression() { }

        public override String Print (int depth)
        {
            return "";
        }

        public override void Visit (Visitor v)
        {
            v.VisitExpr(this);
        }
    }
}
