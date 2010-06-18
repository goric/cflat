using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTMultiply : ASTBinary
    {
        public ASTMultiply (ASTExpression left, ASTExpression right)
            : base(left, right)
        {
        }

        public override String Print(int depth)
        {
            return "(" + Left.Print(depth) + " * " + Right.Print(depth) + ")";
        }

        public override void Visit (Visitor v)
        {
            v.VisitMul(this);
        }
    }
}
