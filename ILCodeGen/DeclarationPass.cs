using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;

namespace ILCodeGen
{
    /// <summary>
    /// This class visits all declarations for constructor, fields, and methods and adds them to the TypeManger
    /// </summary>
    public class DeclarationPass : ClassPass
    {
        private string _currentType;

        public DeclarationPass(TypeManager m)
            : base(m)
        {
            _currentType = "";
        }

        /// <summary>
        /// We need to track the class name that we're visiting, so we can lookup the entry in
        /// TypeManager, so we can access the TypeBuilder and other fun stuff.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            _currentType = n.Name;
            n.Declarations.Visit(this);
        }

        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            _currentType = n.Name;
            n.Declarations.Visit(this);
        }
        
        public override void VisitDeclMethod(ASTDeclarationMethod n)
        {
            _mgr.AddMethod(_currentType, n);
        }

        public override void VisitDeclField(ASTDeclarationField n)
        {
            _mgr.AddField(_currentType, n);
        }

        public override void VisitDeclConstructor(ASTDeclarationCtor n)
        {
            _mgr.AddCtor(_currentType, n);
        }
    }
}
