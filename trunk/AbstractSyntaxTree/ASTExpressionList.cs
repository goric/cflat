using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTExpressionList : ASTExpression
    {
        public bool IsEmpty { get; set; }
        public ASTExpression Expr { get; set; }
        public ASTExpressionList Tail { get; set; }
        public int Length { get; set; }

        public ASTExpressionList ()
        {
            IsEmpty = true;
            Length = 0;
        }
        public ASTExpressionList(ASTExpression exp, ASTExpressionList tail)
        {
            Expr = exp;
            Tail = tail;
            Length = Tail.Length + 1;
        }

        public override string Print (int depth)
        {
            if (IsEmpty)
                return "";

            if (Tail.IsEmpty)
                return Expr.Print(depth).ToString();
            else
                return Expr.Print(depth) + "," + Tail.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitExprList(this);
        }
    }
}
