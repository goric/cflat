using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTBinary : ASTExpression
    {
        public ASTExpression Left { get; set; }
        public ASTExpression Right { get; set; }

        public ASTBinary(ASTExpression left, ASTExpression right)
        {
            Left = left;
            Right = right;
        }

        public override void Visit (Visitor v)
        {
            v.VisitBinary(this);
        }
    }
}
