using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTNegative : ASTExpression
    {
        public ASTExpression Expression { get; set; }
        
        public ASTNegative (ASTExpression exp)
        {
            Expression = exp;
        }

        public override String Print (int depth)
        {
            return " - " + Expression.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitNeg(this);
        }
    }
}
