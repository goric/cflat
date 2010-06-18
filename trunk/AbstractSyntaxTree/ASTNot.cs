using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTNot : ASTExpression
    {
        public ASTExpression Expression { get; set; }

        public ASTNot (ASTExpression not)
        {
            Expression = not;
        }

        public override String Print (int depth)
        {
            return "!" + Expression.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitNot(this);
        }
    }
}
