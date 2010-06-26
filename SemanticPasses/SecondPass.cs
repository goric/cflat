using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    public class SecondPass : FirstPass
    {
        private ActualBuilder _actuals;
        private FormalBuilder _formals;
        private TypeClass _currentClass;

        public SecondPass(ASTNode treeNode)
            : base(treeNode)
        {

        }

        /// <summary>
        /// Define class-level scope --- per class
        /// </summary>
        /// <param name="n"></param>
        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            _currentClass = (TypeClass)n.ClassDesc.Type;
            _scopeMgr.CurrentScope = new Scope(string.Format("class {0}", _currentClass.ClassName), _scopeMgr.CurrentScope);
            this.VisitDeclList(n.Declarations);
            n.Type = _currentClass;
        }

        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            _currentClass = (TypeClass)n.Descriptor.Type;
            _scopeMgr.CurrentScope = new Scope(string.Format("class {0}", _currentClass.ClassName), /*need to get parent scope*/ null);
            this.VisitDeclList(n.Decls);
            n.Type = _currentClass;
        }

        
        
    }
}
