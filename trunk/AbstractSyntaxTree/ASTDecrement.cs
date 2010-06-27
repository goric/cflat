using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTDecrement : ASTExpression
    {
        public ASTExpression Expression { get; set; }

        public ASTDecrement (ASTExpression dec)
        {
            Expression = dec;
        }

        public override String Print (int depth)
        {
            return Expression.Print(depth) + "--";
        }

        public override void Visit (Visitor v)
        {
            v.VisitDecrement(this);
        }
    }
}
