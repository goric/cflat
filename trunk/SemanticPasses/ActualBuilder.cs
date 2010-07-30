using System.Collections.Generic;
using AbstractSyntaxTree;
using SemanticAnalysis;
using System.Linq;

namespace CFlat.SemanticPasses
{
    public class ActualBuilder : AbstractSyntaxTree.Visitor
    {
        //Public stuff like this is sloppy but w/e
        public List<ActualDescriptor> Actuals;
        private MethodDescriptor _methodDesc;

        public ActualBuilder ()
        {
            Actuals = new List<ActualDescriptor>();
        }
        public ActualBuilder(MethodDescriptor methodDesc)
        {
            Actuals = new List<ActualDescriptor>();
            _methodDesc = methodDesc;
        }

        public override void VisitExprList(ASTExpressionList n)
        {
            if (!n.IsEmpty)
            {
                ActualDescriptor actual;

                if (n.Expr is ASTIdentifier)
                    actual = new ActualDescriptor(n.Expr.CFlatType, ((ASTIdentifier)n.Expr).ActualModifier, ((ASTIdentifier)n.Expr).ID);
                else
                    actual = new ActualDescriptor(n.Expr.CFlatType);

                Actuals.Add(actual);
                n.Tail.Visit(this);
            }
            else
            {
                if(_methodDesc != null)
                    FindActualsFromFormals();
            }
        }

        // if any of the actuals were passed into the current function as formals, 
        // we need to know so we can do readonly checks
        private void FindActualsFromFormals ()
        {
            foreach (var actual in Actuals)
            {
                var formalMatch = _methodDesc.Formals.Where(p => p.Name.Equals(actual.Name, System.StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

                if (formalMatch == null)
                    continue;

                actual.IsFromFormal = true;

                if(!string.IsNullOrEmpty(formalMatch.Modifier)
                    && formalMatch.Modifier.Equals("readonly", System.StringComparison.OrdinalIgnoreCase))
                    actual.IsFormalReadonly = true;
            }
        }
    }
}
