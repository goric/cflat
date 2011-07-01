using System;
using AbstractSyntaxTree;

namespace ILCodeGen
{
    /// <summary>
    /// This class collects all types and creates the TypeBuilders
    /// </summary>
    public class ClassPass : Visitor
    {
        protected TypeManager _mgr;

        public ClassPass(TypeManager m)
        {
            _mgr = m;
        }

        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            _mgr.AddClass(n);
        }

        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            _mgr.AddSubClass(n);
        }

        public void Run(AbstractSyntaxTree.ASTNode n)
        {
            n.Visit(this);
        }
    }
}
