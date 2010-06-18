using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTAnd : ASTBinary
    {
        public ASTAnd (ASTExpression exp1, ASTExpression exp2)
            : base(exp1, exp2)
        {
        }

        public override String Print (int depth)
        {
            return Left.Print(depth) + " && " + Right.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitAnd(this);
        }
    }
}
