using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ILCodeGen
{
    /// <summary>
    /// This class adds method descriptors to the classes that contain the methods
    /// </summary>
    public class MethodPass : ClassPass
    {
        private string _currentType;

        public MethodPass(TypeManager m) : base(m)
        {
            _currentType = "";
        }

        public override void VisitClassDefinition(AbstractSyntaxTree.ASTClassDefinition n)
        {
            _currentType = n.Name;

            n.Declarations.Visit(this);
        }

        public override void VisitSubClassDefinition(AbstractSyntaxTree.ASTSubClassDefinition n)
        {
            _currentType = n.Name;

            n.Declarations.Visit(this);
        }

        /// <summary>
        /// Add this entire method to the class it's in
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclMethod(AbstractSyntaxTree.ASTDeclarationMethod n)
        {
            _mgr.MethodMap.Add(_currentType, n);
        }
    }
}
