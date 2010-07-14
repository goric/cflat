using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    public class ActualBuilder : AbstractSyntaxTree.Visitor
    {
        //Public stuff like this is sloppy but w/e
        public List<CFlatType> Actuals;

        public ActualBuilder()
        {
            Actuals = new List<CFlatType>();
        }

        public override void VisitExprList(ASTExpressionList n)
        {
            Actuals.Add(n.Expr.CFlatType);
            n.Tail.Visit(this);
        }
    }
}
