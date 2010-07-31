using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTVoidExpression : ASTExpression
    {
        public override void Visit(Visitor v)
        {
            v.VisitVoidExpr(this);
        }
    }
}
