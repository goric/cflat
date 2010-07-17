using System.Collections.Generic;
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
            if (!n.IsEmpty)
            {
                Actuals.Add(n.Expr.CFlatType);
                n.Tail.Visit(this);
            }
        }
    }
}
