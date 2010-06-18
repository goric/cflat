using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTOr : ASTBinary
    {
        public ASTOr (ASTExpression left, ASTExpression right)
            : base(left, right)
        {
        }

        public override String Print (int depth)
        {
            return Left.Print(depth) + " || " + Right.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitOr(this);
        }
    }
}
