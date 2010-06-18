using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTAssign : ASTStatement
    {
        private ASTExpression LValue { get; set; }
        private ASTExpression Expr { get; set; }

        public ASTAssign(ASTExpression lval, ASTExpression exp)
        {
            LValue = lval;
            Expr = exp;
        }

        public override String Print (int depth)
        {
            return LValue.Print(depth) + " = " + Expr.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitAssign(this);
        }
    }
}
