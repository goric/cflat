using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTIncrement : ASTExpression
    {
        public ASTExpression Expression { get; set; }

        public ASTIncrement (ASTExpression inc)
        {
            Expression = inc;
        }

        public override String Print (int depth)
        {
            return Expression.Print(depth) + "++";
        }

        public override void Visit (Visitor v)
        {
            v.VisitIncrement(this);
        }
    }
}
