using AbstractSyntaxTree;
using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    /// <summary>
    /// I think this is all this class needs to do...
    /// </summary>
    public class FormalBuilder : AbstractSyntaxTree.Visitor
    {
        private TypeFunction _function;

        public FormalBuilder(TypeFunction f)
        {
            _function = f;
        }

        public override void VisitFormal(ASTFormal n)
        {
            _function.AddFormal(n.Name, n.CFlatType);
        }
    }
}
