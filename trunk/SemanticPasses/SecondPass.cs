using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;

using QUT.Gppg;

namespace CFlat.SemanticPasses
{
    /// <summary>
    /// The second semantic pass does type checking and stores information on declarations.  
    /// This includes classes, member variables, and methods within the classes.  Also creates
    /// variable scopes for each class and method for validation later.
    /// </summary>
    public class SecondPass : FirstPass, ICompilerPass
    {
        protected const string READONLY_MODIFIER = "readonly";
        protected const string NECESSARY_MODIFIER = "necessary";
        protected const string PRIVATE_MODIFIER = "private";

        public SecondPass(ASTNode treeNode, ScopeManager mgr)
            : base(treeNode, mgr)
        {
            
        }

        new public string PassName()
        {
            return "Second Semantic Pass";
        }

        /// <summary>
        /// Define class-level scope --- per class
        /// </summary>
        /// <param name="n"></param>
        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            _currentClass = new TypeClass(n.Name);
            var classScope = _scopeMgr.PushScope(string.Format("class {0}", _currentClass.ClassName));

            _currentClass.Descriptor = (ClassDescriptor)_scopeMgr.Find(n.Name, p => p is ClassDescriptor);
            
            var declarationType = CheckSubTree(n.Declarations);
            n.Type = _currentClass;

            _currentClass.Descriptor.Scope = _currentClass.Scope = classScope;

            AddCtorIfNone(classScope, n.Name);

            _scopeMgr.PopScope();
        }

        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            var parent = (ClassDescriptor)_scopeMgr.Find(n.Parent, p => p is ClassDescriptor);
            //need to restore the parent class's scope so that inheritance can pass semantic analysis
            _scopeMgr.RestoreScope(parent.Scope);

            _currentClass = new TypeClass(n.Name, parent);
            var classScope = _scopeMgr.PushScope(string.Format("subclass {0}", _currentClass.ClassName));

            _currentClass.Descriptor = (ClassDescriptor)_scopeMgr.Find(n.Name, p => p is ClassDescriptor);

            var declarationType = CheckSubTree(n.Declarations);
            n.Type = _currentClass;

            CheckNecessaryFunctions(n);

            _currentClass.Descriptor.Scope = _currentClass.Scope = classScope;

            AddCtorIfNone(classScope, n.Name);

            //pop the sub class scope and the parent class
            _scopeMgr.PopScope();
            _scopeMgr.PopScope();
        }

        private void AddCtorIfNone (Scope classScope, string name)
        {
            var ctor = _currentClass.Descriptor.Methods.Where(p => p.Name.Equals(_currentClass.ClassName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();
            if (ctor == null)
            {
                var func = new TypeFunction(name) { ReturnType = new TypeVoid(), IsConstructor = true, Scope = classScope };
                _scopeMgr.AddMethod(name, func, _currentClass);
            }
        }

        //finds any 'necessary' methods in the parent class and makes sure that this subclass implements them
        private void CheckNecessaryFunctions (ASTSubClassDefinition n)
        {
            var parent = (ClassDescriptor)_scopeMgr.Find(n.Parent, p => p is ClassDescriptor);

            foreach(var method in parent.Methods)
                if (method.Modifiers.Contains(NECESSARY_MODIFIER, StringComparer.InvariantCultureIgnoreCase))
                    if (!_scopeMgr.CurrentScope.HasSymbol(method.Name))
                        ReportError(n.Location, "Class '{0}' does not implement method '{1}', marked necessary by superclass '{2}'", n.Name, method.Name, n.Parent);
        }

        /// <summary>
        /// Type-check and gather information about member variables.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclField (ASTDeclarationField n)
        {
            var declFieldType = CheckSubTree(n.FieldType);
            n.Type = declFieldType;

            var mods = GatherFieldModifiers(n);

            var desc = _scopeMgr.AddMember(n.Name, declFieldType, _currentClass, mods);
            n.Descriptor = desc;
        }

        private List<string> GatherFieldModifiers (ASTDeclarationField n)
        {
            var names = new List<String>();
            var mods = n.Modifiers;
            while (!mods.IsEmpty)
            {
                names.Add(mods.Modifier);
                mods = mods.Tail;
            }
            return names;
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
            var func = new TypeFunction(n.Name);

            var formalDescriptors = CollectFormals(n.Formals, func);

            var returnType = CheckSubTree(n.ReturnType);
            func.ReturnType = returnType;
            func.Scope = methodScope;

            var mods = GatherModifiers(n);

            _scopeMgr.PopScope();
            var methodDesc = _scopeMgr.AddMethod(n.Name, func, _currentClass, mods);
            n.Descriptor = methodDesc;
            n.Type = func;

            foreach (var formal in formalDescriptors)
                methodDesc.Formals.Add(formal);
        }

        private List<string> GatherModifiers (ASTDeclarationMethod n)
        {
            var names = new List<String>();
            var mods = n.Modifiers;
            while (!mods.IsEmpty)
            {
                names.Add(mods.Modifier);
                mods = mods.Tail;
            }
            return names;
        }

        /// <summary>
        /// Essentially the same as a method declaration, except there is no
        /// return type to validate.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclConstructor (ASTDeclarationCtor n)
        {
            var ctorScope = _scopeMgr.PushScope(string.Format("ctor {0}", n.Name));
            var func = new TypeFunction(n.Name) { ReturnType = new TypeVoid(), IsConstructor = true };

            var formalDescriptors = CollectFormals(n.Formals, func);

            func.Scope = ctorScope;
            _scopeMgr.PopScope();
            var methodDesc = _scopeMgr.AddMethod(n.Name, func, _currentClass);
            n.Descriptor = methodDesc;
            n.Type = func;

            foreach (var formal in formalDescriptors)
                methodDesc.Formals.Add(formal);
        }

        private List<FormalDescriptor> CollectFormals (ASTFormalList formals, TypeFunction containingFunction)
        {
            var descList = new List<FormalDescriptor>();
            var formalsType = CheckSubTree(formals);

            if (!formals.IsEmpty)
            {
                var list = formals;
                while (!list.IsEmpty)
                {
                    containingFunction.AddFormal(list.Formal.Name, list.Formal.CFlatType);
                    descList.Add(new FormalDescriptor(list.Formal.CFlatType, list.Formal.Name, list.Formal.Modifier));
                    list = list.Tail;
                }
            }

            return descList;
        }

        public override void VisitFormal(ASTFormal n) 
        {
            n.CFlatType = CheckSubTree(n.Type);
		    _scopeMgr.AddFormal(n.Name, n.CFlatType, n.Modifier);
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

        public override void VisitTypeVoid (ASTTypeVoid n)
        {
            n.Type = _lastSeenType = new TypeVoid();
        }

        public override void VisitTypeName (ASTTypeClass n)
        {
            n.Type = _lastSeenType = new TypeClass(n.Name);
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
