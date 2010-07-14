using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    /// <summary>
    /// The second semantic pass does type checking and stores information on declarations.  
    /// This includes classes, member variables, and methods within the classes.  Also creates
    /// variable scopes for each class and method for validation later.
    /// </summary>
    public class SecondPass : FirstPass
    {
        private ActualBuilder _actuals;
        private FormalBuilder _formals;

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
            _currentClass = (TypeClass)n.Descriptor.Type;
            var classScope = _scopeMgr.PushScope(string.Format("class {0}", _currentClass.ClassName));

            var declarationType = CheckSubTree(n.Declarations);
            n.Type = _currentClass;

            _currentClass.Scope = classScope;
            _scopeMgr.PopScope();
        }

        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            _currentClass = (TypeClass)n.Descriptor.Type;
            var classScope = _scopeMgr.PushScope(string.Format("subclass {0}", _currentClass.ClassName));

            var declarationType = CheckSubTree(n.Declarations);
            n.Type = _currentClass;

            _currentClass.Scope = classScope;
            _scopeMgr.PopScope();
        }

        /// <summary>
        /// Type-check and gather information about member variables.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclField (ASTDeclarationField n)
        {
            var declFieldType = CheckSubTree(n.FieldType);
            n.Type = declFieldType;
            var desc = _scopeMgr.AddMember(n.Name, declFieldType, _currentClass);
            n.Descriptor = desc;
        }

        /// <summary>
        /// Type check and gather top-level information about a method.  This includes
        /// checking the return type, formal parameter types, and creating a scope for the 
        /// method.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclMethod (ASTDeclarationMethod n)
        {
            var methodScope = _scopeMgr.PushScope(string.Format("method {0}", n.Name));
            var func = new TypeFunction();

            CollectFormals(n.Formals, func);

            var returnType = CheckSubTree(n.ReturnType);
            func.ReturnType = returnType;
            func.Scope = methodScope;

            _scopeMgr.PopScope();
            var methodDesc = _scopeMgr.AddMethod(n.Name, func, _currentClass);
            n.Descriptor = methodDesc;
            n.Type = func;
        }

        /// <summary>
        /// Essentially the same as a method declaration, except there is no
        /// return type to validate.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclConstructor (ASTDeclarationCtor n)
        {
            var ctorScope = _scopeMgr.PushScope(string.Format("ctor {0}", n.Name));
            var func = new TypeFunction() { ReturnType = new TypeVoid(), IsConstructor = true };

            CollectFormals(n.Formals, func);

            func.Scope = ctorScope;
            _scopeMgr.PopScope();
            var methodDesc = _scopeMgr.AddMethod(n.Name, func, _currentClass);
            n.Descriptor = methodDesc;
            n.Type = func;
        }

        private void CollectFormals (ASTFormalList formals, TypeFunction containingFunction)
        {
            var formalsType = CheckSubTree(formals);

            if (!formals.IsEmpty)
            {
                var list = formals;
                while (!list.IsEmpty)
                {
                    containingFunction.AddFormal(list.Formal.Name, list.Formal.CFlatType);
                    list = list.Tail;
                }
            }
        }

        public override void VisitFormal(ASTFormal n) 
        {
            n.CFlatType = CheckSubTree(n.Type);
		    _scopeMgr.AddFormal(n.Name, n.CFlatType);
        }

        public override void VisitTypeArray (ASTTypeArray n)
        {
            _lastSeenType = CheckSubTree(n.BaseType);
            n.Type = _lastSeenType = new TypeArray(_lastSeenType);
        }

        public override void VisitTypeBool (ASTTypeBool n)
        {
            n.Type = _lastSeenType = new TypeBool();
        }

        public override void VisitTypeInt (ASTTypeInt n)
        {
            n.Type = _lastSeenType = new TypeInt();
        }

        public override void VisitTypeName (ASTTypeName n)
        {
            Descriptor aDesc = _scopeMgr.GetType(n.Name);
            n.Type = _lastSeenType = new TypeName(aDesc.Type);
        }

        public override void VisitTypeVoid (ASTTypeVoid n)
        {
            n.Type = _lastSeenType = new TypeVoid();
        }

        public override void VisitTypeReal (ASTTypeReal n)
        {
            n.Type = _lastSeenType = new TypeReal();
        }

        public override void VisitTypeString (ASTTypeString n)
        {
            n.Type = _lastSeenType = new TypeString();
        }
    }
}
