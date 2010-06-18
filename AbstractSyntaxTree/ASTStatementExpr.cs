using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTStatementExpr : ASTStatement
    {
        public ASTExpression Expression { get; set; }

        public ASTStatementExpr (ASTExpression exp)
        {
            Expression = exp;
        }

        public override String Print (int depth)
        {
            return Expression.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitStatementExpr(this);
        }
    }
}
