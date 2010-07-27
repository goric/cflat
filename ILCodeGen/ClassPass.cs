using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILCodeGen
{
    /// <summary>
    /// This class collects all types and builds an inheritance hierarchy
    /// </summary>
    public class ClassPass : AbstractSyntaxTree.Visitor
    {
        protected TypeManager _mgr;

        public ClassPass(TypeManager m)
        {
            _mgr = m;
        }

        public override void VisitClassDefinition(AbstractSyntaxTree.ASTClassDefinition n)
        {
            _mgr.InheritanceMap.Add(n.Name, "");
        }

        public override void VisitSubClassDefinition(AbstractSyntaxTree.ASTSubClassDefinition n)
        {
            _mgr.InheritanceMap.Add(n.Name, n.Parent);
        }

        public void Run(AbstractSyntaxTree.ASTNode n)
        {
            n.Visit(this);
        }
    }
}
