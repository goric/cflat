using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using System.Reflection.Emit;
using System.Reflection;
using SemanticAnalysis;


namespace ILCodeGen
{
    /// <summary>
    /// This class generates CIL by walking a syntax tree.  This assumes that semantic checking has already
    /// been done.  Single Pass for now.
    /// TODO:Needs to be multiple-pass, can't invoke a method until it's defined, can't create a type until its methods are defined...
    /// weird
    /// </summary>
    public class CodeGenerator : Visitor
    {
        protected string _assemblyName;
        protected TypeBuilder _currentType;
        protected AssemblyBuilder _asm;
        protected ModuleBuilder _mod;
        protected ILGenerator _gen;
        protected Type _lastWalkedType;
        protected string _lastWalkedIdentifier;
        protected Dictionary<string, int> _locals;
        protected TypeManager _mgr;
        private Stack<Type> _typesOnStack;
        private List<ASTFormal> _tmpFormals;
        private bool _isArrayAssign;
        
        //i heard you like dictionaries, so i put a dictionary in your dictionary
        private Dictionary<string, Dictionary<string, MethodBuilder>> _methods;

        private MethodAttributes _tmpAttr;

        private const string MAIN_METHOD_NAME = "main";

        public CodeGenerator(string assemblyName, TypeManager m)
            : base()
        {
            _mgr = m;
            _assemblyName = assemblyName;
            _locals = new Dictionary<string, int>();
            _methods = new Dictionary<string, Dictionary<string, MethodBuilder>>(); ;
            _typesOnStack = new Stack<Type>();
            
            
            Init();
        }

        private void Init()
        {
            //Initialize assembly
            _asm = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(_assemblyName),
                AssemblyBuilderAccess.RunAndSave);
            //Create Module
            string exeName = _assemblyName + ".exe";
            _mod = _asm.DefineDynamicModule(exeName, exeName);

            DefineClasses();

            DefineMethodStubs();

            //DefineTypes();
        }

        public void Generate(ASTNode n)
        {
            n.Visit(this);

            DefineTypes();
        }

        #region Declare/Define
        
        private void DefineClasses()
        {
            foreach (string className in _mgr.InheritanceMap.Keys)
            {
                if (!_mgr.CFlatTypes.ContainsKey(className))
                {
                    if (!String.IsNullOrEmpty(_mgr.InheritanceMap[className]))
                    {
                        GetParent(className);
                    }
                    else
                    {
                        //TODO:Access Modifiers
                        _mgr.CFlatTypes.Add(className, _mod.DefineType(className, TypeAttributes.Class | TypeAttributes.Public));
                    }
                }
            }
        }

        private void GetParent(string className)
        {
            //go back up inheritance tree - this will make sure that the parent is defined before the child class
            if(_mgr.InheritanceMap.ContainsKey(_mgr.InheritanceMap[className]))
            {
                GetParent(_mgr.InheritanceMap[className]);
            }

            //add type - add parent if it is not null
            if (String.IsNullOrEmpty(_mgr.InheritanceMap[className]))
            {
                if(!_mgr.CFlatTypes.ContainsKey(className))
                    _mgr.CFlatTypes.Add(className, _mod.DefineType(className, TypeAttributes.Class | TypeAttributes.Public));
            }
            else
            {
                if(!_mgr.CFlatTypes.ContainsKey(className))
                    _mgr.CFlatTypes.Add(className, _mod.DefineType(className, TypeAttributes.Public | TypeAttributes.Class,
                        _mgr.CFlatTypes[_mgr.InheritanceMap[className]]));
            }
        }

        //This stubs out all the methods so you don't get odd invoke errors
        //it should also let code like this compile --- public void a() { b(); } public void b() { a(); }
        private void DefineMethodStubs()
        {
            foreach (TypeBuilder tb in _mgr.CFlatTypes.Values)
            {
                _methods.Add(tb.Name, new Dictionary<string, MethodBuilder>());

                foreach(AbstractSyntaxTree.ASTDeclarationMethod decl in _mgr.MethodMap[tb.Name])
                {
                    //TODO:Add Access Modifiers
                    MethodAttributes attr = MethodAttributes.Public |
                        MethodAttributes.Static |
                        MethodAttributes.HideBySig;

                    
                    MethodBuilder mb = tb.DefineMethod(decl.Name,
                        attr);

                    //Define method signature - once an invoke is run, you can no longer change method signatures
                    _tmpFormals = new List<ASTFormal>();
                    decl.Formals.Visit(this);

                    decl.ReturnType.Visit(this);

                    List<Type> formalTypes = new List<Type>();
                    formalTypes.AddRange(_tmpFormals.ConvertAll<Type>(m => { return GetCilType(m.CFlatType); }));

                    mb.SetParameters(formalTypes.ToArray());

                    mb.SetReturnType(_lastWalkedType);
                    
                    _methods[tb.Name].Add(decl.Name, mb);

                    //string prnt = _mgr.InheritanceMap[tb.Name];
                    //if(!String.IsNullOrEmpty(prnt))
                    //{
                    //    if(_methods[prnt].ContainsKey(decl.Name))
                    //        tb.DefineMethodOverride(mb, _methods[prnt][decl.Name]);
                    //}
                    //_methods[tb.Name][decl.Name].GetILGenerator().Emit(OpCodes.Nop);
                }
            }
        }

        public void DefineTypes()
        {
            foreach(TypeBuilder tb in _mgr.CFlatTypes.Values)
            {
                tb.CreateType();
            }
        }

        public override void VisitClassDefinition(ASTClassDefinition n)
        {       
            //gen type
            //_currentType = _mod.DefineType(n.Name, TypeAttributes.Class | TypeAttributes.Public);
            _currentType = _mgr.CFlatTypes[n.Name];

            n.Declarations.Visit(this);

            //_currentType.CreateType();
                               
        }

        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            //TODO: need to get parent type
            //_currentType = _mod.DefineType(n.Name, TypeAttributes.Class | TypeAttributes.Public);
            _currentType = _mgr.CFlatTypes[n.Name];

            n.Declarations.Visit(this);

            //_currentType.CreateType();

        }

        public override void VisitModifierList(ASTModifierList n)
        {
            //bitwise OR what we have with the next modifier in the list
            //handled by this switch statement...
            switch (n.Modifier)
            {
                case "public":
                    _tmpAttr |= MethodAttributes.Public;
                    break;
                case "static":
                    _tmpAttr |= MethodAttributes.Static;
                    break;
                case "private":
                    _tmpAttr |= MethodAttributes.Private;
                    break;
                case "final":
                    _tmpAttr |= MethodAttributes.Final;
                    break;
                case "abstract":
                    _tmpAttr |= MethodAttributes.Abstract;
                    break;
                default:
                    _tmpAttr |= MethodAttributes.Public | MethodAttributes.Static;
                    break;
            }
            
        }

        public override void VisitFormal(ASTFormal n)
        {
            n.Type.Visit(this);
            _tmpFormals.Add(n);
        }

        public override void VisitDeclMethod(ASTDeclarationMethod n)
        {
            n.Modifiers.Visit(this);
                        
            MethodBuilder meth = _methods[_currentType.Name][n.Name];
            
            //meth.DefineParameter(0, ParameterAttributes.Retval, String.Empty);
            
            //set il generator to this method
            _gen = meth.GetILGenerator();

            //is this the entry point
            if (n.Name == MAIN_METHOD_NAME)
            {
                _asm.SetEntryPoint(meth);
            }
            
            //reset attrs
            _tmpAttr = 0;

            //seems silly to visit this just to get count, but it's ok
            _tmpFormals = new List<ASTFormal>();
            n.Formals.Visit(this);         
            
            //put formals into local storage
            foreach (ASTFormal f in _tmpFormals)
            {
                _gen.Emit(OpCodes.Ldarg, _tmpFormals.IndexOf(f));

                LocalBuilder lb = _gen.DeclareLocal(GetCilType(f.CFlatType));
                StoreLocal(f.Name, lb.LocalIndex);
            }

            n.Body.Visit(this);

            //return
            _gen.Emit(OpCodes.Ret);

            //put this back?
            _methods[_currentType.Name][n.Name] = meth;
        }

        public override void VisitDeclLocal(ASTDeclarationLocal n)
        {
            //Get type
            n.Type.Visit(this);

            LocalBuilder lb = _gen.DeclareLocal(_lastWalkedType);

            //set value
            if(n.InitialValue != null)
                n.InitialValue.Visit(this);

            StoreLocal(n.ID, lb.LocalIndex);
            
        }
        public override void VisitDeclField(ASTDeclarationField n)
        {
            //TODO: Check Modifier list
            FieldBuilder fb = _currentType.DefineField(n.Name, _lastWalkedType, FieldAttributes.Public);
            
        }
        public override void VisitDeclConstructor(ASTDeclarationCtor n)
        {
           n.Formals.Visit(this);
           List<Type> formalTypes = new List<Type>();
           formalTypes.AddRange(_tmpFormals.ConvertAll<Type>(m => { return GetCilType(m.CFlatType); }));

           ConstructorBuilder cb = _currentType.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, formalTypes.ToArray());
         
            
        }
        public override void VisitInstantiateArray (ASTInstantiateArray n)
        {
            //push upper
            n.Upper.Visit(this);
            
            //get the type and push newarr [type]
            n.Type.Visit(this);
            _gen.Emit(OpCodes.Newarr, _lastWalkedType);
        }

        /// <summary>
        /// Get class, instantiate and call constructor
        /// </summary>
        /// <param name="n"></param>
        public override void VisitInstantiateClass(ASTInstantiateClass n)
        {
            n.Actuals.Visit(this);

            TypeBuilder tb = _mgr.CFlatTypes[n.ClassName];

            List<Type> types = _typesOnStack.Take(n.Actuals.Length).ToList();
            types.Reverse();

            //not sure this is right
            _gen.Emit(OpCodes.Newobj, tb.GetConstructor(types.ToArray()));

        }

        public override void VisitReturn(ASTReturn n)
        {
            n.ReturnValue.Visit(this);

            _gen.Emit(OpCodes.Ret);
        }
        #endregion

        #region control/invoke

        public override void VisitInvoke(ASTInvoke n)
        {
            
            n.Actuals.Visit(this);

            //for(int i = n.Actuals.Length; i > 0; i--)
            //{
            //    _gen.Emit(OpCodes.Starg, i - 1);
            //}

            List<Type> types = _typesOnStack.Take(n.Actuals.Length).ToList();
            types.Reverse();

            if (n.Method == "println")
            {
                _gen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static,
                    null, types.ToArray(), null));
            }
            else if (n.Method == "print")
            {
                _gen.Emit(OpCodes.Call, typeof(Console).GetMethod("Write", BindingFlags.Public | BindingFlags.Static,
                    null, types.ToArray(), null));
            }
            else if (n.Method == "readln")
            {
                _gen.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine", BindingFlags.Public | BindingFlags.Static,
                    null, System.Type.EmptyTypes, null));
                _typesOnStack.Push(typeof(string));
                //_gen.Emit(OpCodes.Stloc_0);
            }
            else if (n.Method == "parseInt")
            {
                _gen.Emit(OpCodes.Call, typeof(Int32).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static,
                    null, types.ToArray(), null));
                _typesOnStack.Push(typeof(int));
            }
            else
            {
                TypeBuilder b4 = _currentType;
                n.Object.Visit(this);

                //MethodInfo mi = _methods[_currentType.Name][n.Method].GetBaseDefinition();
                MethodInfo mi = _methods[_currentType.Name][n.Method];
                //mi = _methods[_currentType.Name][n.Method].GetGenericMethodDefinition();

                //mi = _methods[_currentType.Name][n.Method].MakeGenericMethod(_methods[_currentType.Name][n.Method].GetGenericArguments());
                //mi.GetGenericArguments();                                

                _gen.Emit(OpCodes.Call, mi);

                _currentType = b4;
            }
            
           
        }

        public override void VisitBase(ASTBase n)
        {
            _currentType = _mgr.CFlatTypes[_mgr.InheritanceMap[_currentType.Name]];
        }
        

        public override void VisitIfThen(ASTIfThen n)
        {
            Label skipLabel = _gen.DefineLabel();

            //evaluate
            n.Condition.Visit(this);

            //skip on false
            _gen.Emit(OpCodes.Brfalse, skipLabel);

            //walk statements
            n.Then.Visit(this);

            //mark label
            _gen.MarkLabel(skipLabel);
        }

        public override void VisitIfThenElse(ASTIfThenElse n)
        {
            Label elseLabel = _gen.DefineLabel();
            Label jumpOver = _gen.DefineLabel();

            //check condition
            n.Condition.Visit(this);

            //go to else on false
            _gen.Emit(OpCodes.Brfalse, elseLabel);

            //walk if
            n.Then.Visit(this);
            _gen.Emit(OpCodes.Br, jumpOver);

            //transfer control here, then walk else body
            _gen.MarkLabel(elseLabel);
            //walk else statments
            n.Else.Visit(this);

            _gen.MarkLabel(jumpOver);
        }

        public override void VisitFor(ASTFor n)
        {
            //define labels
            Label loop = _gen.DefineLabel();
            Label exit = _gen.DefineLabel();

            //evaluate initial expression
            n.InitialExpr.Visit(this);

            //loop label
            _gen.MarkLabel(loop);

            //check condition
            n.Conditional.Visit(this);

            //break on false
            _gen.Emit(OpCodes.Brfalse, exit);

            //emit body of loop
            n.Body.Visit(this);

            //evaluate looping expression
            n.LoopExpr.Visit(this);

            //unconditional loop branch
            _gen.Emit(OpCodes.Br, loop);

            //break label
            _gen.MarkLabel(exit);
        }

        public override void VisitForIn(ASTForIn n)
        {
            //define labels
            Label loop = _gen.DefineLabel();
            Label exit = _gen.DefineLabel();

            //evaluate lower
            n.Lower.Visit(this);

            LocalBuilder lb = _gen.DeclareLocal(typeof(int));

            //store temp variable
            StoreLocal(n.TempVariable.ID, lb.LocalIndex);
            
            //loop label
            _gen.MarkLabel(loop);

            //load value into temp variable - set type so GetCilType()
            //doesn't get confused
            n.TempVariable.CFlatType = new TypeInt();
            n.TempVariable.Visit(this);

            //visit upper bound
            n.Upper.Visit(this);

            _gen.Emit(OpCodes.Cgt);

            //break on true
            _gen.Emit(OpCodes.Brtrue, exit);

            //emit body of loop
            n.Body.Visit(this);

            n.TempVariable.Visit(this);
            _gen.Emit(OpCodes.Ldc_I4_1);
            _gen.Emit(OpCodes.Add);

            StoreLocal(n.TempVariable.ID, lb.LocalIndex);

            //unconditional loop branch
            _gen.Emit(OpCodes.Br, loop);

            //break label
            _gen.MarkLabel(exit);
        }

        public override void VisitWhile(ASTWhile n)
        {
            //define labels
            Label loop = _gen.DefineLabel();
            Label exit = _gen.DefineLabel();

            //loop label
            _gen.MarkLabel(loop);

            //check condition
            n.Condition.Visit(this);

            //break on false
            _gen.Emit(OpCodes.Brfalse, exit);

            //emit body of loop
            n.Body.Visit(this);

            //unconditional loop branch
            _gen.Emit(OpCodes.Br, loop);

            //break label
            _gen.MarkLabel(exit);
        }

        
        #endregion        

        #region types
        public override void VisitTypeInt(ASTTypeInt n)
        {
            _lastWalkedType = GetCilType(n);
        }
        public override void VisitTypeBool(ASTTypeBool n)
        {
            _lastWalkedType = GetCilType(n);
        }
        public override void VisitTypeReal(ASTTypeReal n)
        {
            _lastWalkedType = GetCilType(n);
        }
        public override void VisitTypeString(ASTTypeString n)
        {
            _lastWalkedType = GetCilType(n);
        }
        public override void VisitTypeVoid(ASTTypeVoid n)
        {
            _lastWalkedType = GetCilType(n);
        }
        public override void VisitTypeName(ASTTypeClass n)
        {
            //??
            _lastWalkedType = GetCilType(n);
        }
        public override void VisitTypeArray (ASTTypeArray n)
        {
            _lastWalkedType = GetCilType(n);
        }
        
        #endregion

        #region constants/primitives
        public override void VisitInteger(ASTInteger n)
        {
            //load integer
            _gen.Emit(OpCodes.Ldc_I4, n.Value);
            _typesOnStack.Push(typeof(int));
            _lastWalkedType = typeof(int);
        }
        public override void VisitString(ASTString n)
        {
            //push string on stack
            _gen.Emit(OpCodes.Ldstr, n.Value.Replace("\"", "").Replace("\\n", "\n").Replace("\\t", "\t"));
            _typesOnStack.Push(typeof(string));
            _lastWalkedType = typeof(string);
        }
        public override void VisitBoolean(ASTBoolean n)
        {
            //assume 1 == true?
            if (n.Val) _gen.Emit(OpCodes.Ldc_I4_1); else _gen.Emit(OpCodes.Ldc_I4_0);
            _typesOnStack.Push(typeof(int));
            _lastWalkedType = typeof(int);
        }
        public override void VisitReal(ASTReal n)
        {
            //push real on stack
            _gen.Emit(OpCodes.Ldc_R4, n.Value);
            _typesOnStack.Push(typeof(double));
            _lastWalkedType = typeof(double);
        }
        #endregion

        #region deref
        public override void VisitIdentifier(ASTIdentifier n)
        {
            LoadLocal(n.ID);
            var type = GetCilType(n.CFlatType);
            _typesOnStack.Push(type);
            _lastWalkedType = type;
        }
        public override void VisitDerefArray (ASTDereferenceArray n)
        {
            LoadLocal(((ASTIdentifier)n.Array).ID);
            n.Index.Visit(this);
            _typesOnStack.Push(GetCilType(n.CFlatType));
            //bit of a hack, we need to gen a ldelem on derefences except when its an assignment
            if (!_isArrayAssign)
            {
                if (_lastWalkedType == typeof(int))
                    _gen.Emit(OpCodes.Ldelem_I4);
                if (_lastWalkedType == typeof(double))
                    _gen.Emit(OpCodes.Ldelem, typeof(double));
                if (_lastWalkedType == typeof(bool))
                    _gen.Emit(OpCodes.Ldelem_I4);
                if (_lastWalkedType == typeof(string))
                    _gen.Emit(OpCodes.Ldelem, typeof(string));
            }
        }

        #endregion
        
        #region Binary Operators
        public override void VisitAdd(ASTAdd n)
        {
            SetupOperands(n);
           
            //pop 2 numbers, add, push result
            _gen.Emit(OpCodes.Add);
        }
        public override void VisitSub(ASTSubtract n)
        {
            SetupOperands(n);

            //pop 2, sub, push result
            _gen.Emit(OpCodes.Sub);
        }

        public override void VisitMul(ASTMultiply n)
        {
            SetupOperands(n);

            //pop, multiply, push result
            _gen.Emit(OpCodes.Mul);
        }

        public override void VisitDiv(ASTDivide n)
        {
            SetupOperands(n);

            //pop, divide, push result
            _gen.Emit(OpCodes.Div);
        }

        public override void VisitMod(ASTModulo n)
        {
            SetupOperands(n);

            //divide, push rem on stack
            _gen.Emit(OpCodes.Rem);
        }

        public override void VisitAnd(ASTAnd n)
        {
            SetupOperands(n);

            _gen.Emit(OpCodes.And);
        }

        public override void VisitOr(ASTOr n)
        {
            SetupOperands(n);
        }

        public override void VisitExponent(ASTExponent n)
        {
            string TEMP_NM = "xp_temp";
            string TOP_NM = "xp_top";
            string LOOP_NM = "xp_loop";
            string RES_NM = "xp_res";

            n.Left.Visit(this);
            
            LocalBuilder lb = _gen.DeclareLocal(typeof(int));
            StoreLocal(TEMP_NM, lb.LocalIndex);

            n.Right.Visit(this);
            LocalBuilder lb2 = _gen.DeclareLocal(typeof(int));
            StoreLocal(TOP_NM, lb2.LocalIndex);

            LocalBuilder lb3 = _gen.DeclareLocal(typeof(int));
            _gen.Emit(OpCodes.Ldc_I4_1);
            StoreLocal(LOOP_NM, lb3.LocalIndex);

            LocalBuilder resBuilder = _gen.DeclareLocal(typeof(int));
            LoadLocal(TEMP_NM);
            StoreLocal(RES_NM, resBuilder.LocalIndex);

            //define labels
            Label loop = _gen.DefineLabel();
            Label exit = _gen.DefineLabel();
            
            //loop label
            _gen.MarkLabel(loop);

            //check loop
            LoadLocal(LOOP_NM);
            LoadLocal(TOP_NM);
            _gen.Emit(OpCodes.Bge, exit);

            //load temp and result
            LoadLocal(RES_NM);
            LoadLocal(TEMP_NM);

            //multiply
            _gen.Emit(OpCodes.Mul);

            //store result
            StoreLocal(RES_NM, resBuilder.LocalIndex);

            //load loop var
            LoadLocal(LOOP_NM);
            //add 1
            _gen.Emit(OpCodes.Ldc_I4_1);
            _gen.Emit(OpCodes.Add);

            //update loop var
            StoreLocal(LOOP_NM, lb3.LocalIndex);

            //unconditional loop branch
            _gen.Emit(OpCodes.Br, loop);

            //break label
            _gen.MarkLabel(exit);

            LoadLocal(RES_NM);
        }

        public override void VisitConcatenate (ASTConcatenate n)
        {
            //push left and right
            n.Left.Visit(this);
            BoxIfNecessary(_lastWalkedType);
            n.Right.Visit(this);
            BoxIfNecessary(_lastWalkedType);
            
            //get our parameter types
            List<Type> types = _typesOnStack.Take(2).ToList();
            types.Reverse();

            // now call String.Concat
            _gen.Emit(OpCodes.Call, typeof(String).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static,
               null, types.ToArray(), null));

            //hacky, but we know the last type was a string since we're calling Concat...
            _lastWalkedType = typeof(string);
            _typesOnStack.Push(typeof(string));
        }
        private void BoxIfNecessary (Type t)
        {
            // if not a string we need to box
            if (t != typeof(string))
            {
                if (_lastWalkedType == typeof(int))
                    _gen.Emit(OpCodes.Box, typeof(int));
                else if (_lastWalkedType == typeof(double))
                    _gen.Emit(OpCodes.Box, typeof(double));
            }
        }

        #endregion
        
        #region unary operators
        public override void VisitNeg(ASTNegative n)
        {
            _gen.Emit(OpCodes.Ldc_I4_0);

            n.Expression.Visit(this);

            _gen.Emit(OpCodes.Sub);

        }

        public override void VisitNot(ASTNot n)
        {
            //evaluate expression
            n.Expression.Visit(this);

            //negate it
            _gen.Emit(OpCodes.Not);
        }

        public override void VisitIncrement(ASTIncrement n)
        {
            //get current value - should set the _lastWalked identifier
            n.Expression.Visit(this);
            
            //put 1 on stack
            _gen.Emit(OpCodes.Ldc_I4_1);

            //add 1
            _gen.Emit(OpCodes.Add);
            
            //store back in mem....
            _gen.Emit(OpCodes.Stloc, _locals[_lastWalkedIdentifier]);
            
        }

        public override void VisitDecrement(ASTDecrement n)
        {
            //get current value
            n.Expression.Visit(this);

            //put 1 on stack
            _gen.Emit(OpCodes.Ldc_I4_1);

            //sub 1
            _gen.Emit(OpCodes.Sub);

            //store back in mem....
            _gen.Emit(OpCodes.Stloc, _locals[_lastWalkedIdentifier]);
            
        }
        #endregion

        #region relop
        public override void VisitSmaller(ASTSmaller n)
        {
            //put results on stack
            SetupOperands(n);

            _gen.Emit(OpCodes.Clt);                        
        }

        public override void VisitGreater(ASTGreater n)
        {
            SetupOperands(n);

            _gen.Emit(OpCodes.Cgt);
        }

        public override void VisitEqual(ASTEqual n)
        {
            SetupOperands(n);
            //string equality, good times
            if (_lastWalkedType == typeof(String))
            {
                _gen.Emit(OpCodes.Call, typeof(String).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static, null,
                    new Type[]{ typeof(string), typeof(string) }, null));

            }
            else
            {
                _gen.Emit(OpCodes.Ceq);
            }
        }

        public override void VisitSmallerEqual(ASTSmallerEqual n)
        {
            SetupOperands(n);
            //check eq
            _gen.Emit(OpCodes.Ceq);

            SetupOperands(n);
            //check lt
            _gen.Emit(OpCodes.Clt);

            //or them
            _gen.Emit(OpCodes.Or);
        }

        public override void VisitGreaterEqual(ASTGreaterEqual n)
        {
            SetupOperands(n);
            //check eq
            _gen.Emit(OpCodes.Ceq);

            SetupOperands(n);
            //check gt
            _gen.Emit(OpCodes.Cgt);

            //or them
            _gen.Emit(OpCodes.Or);
        }

        public override void VisitNotEqual(ASTNotEqual n)
        {
            SetupOperands(n);

            //string equality, good times
            if (_lastWalkedType == typeof(String))
            {
                _gen.Emit(OpCodes.Call, typeof(String).GetMethod("Equals", BindingFlags.Public | BindingFlags.Static, null,
                    new Type[]{ typeof(string), typeof(string) }, null));
                                
            }
            else
            {
                _gen.Emit(OpCodes.Ceq);
            }
                        
            _gen.Emit(OpCodes.Not);
        }

        #endregion

        public override void VisitAssign(ASTAssign n)
        {
            //for arrays we need to visit the left side first, then the expression,
            // making sure not to emit a ldelem for the LHS array dereference,
            // and also emit a stelem command
            if (n.LValue is ASTDereferenceArray)
            {
                _isArrayAssign = true;
                n.LValue.Visit(this);
                _isArrayAssign = false;
                n.Expr.Visit(this);
                _gen.Emit(OpCodes.Stelem_I4);
                return;
            }
            n.Expr.Visit(this);
            n.LValue.Visit(this);
        }
        
        public override void VisitStatementExpr(ASTStatementExpr n)
        {
            n.Expression.Visit(this);
        }

        public override void VisitBlock(ASTBlock n)
        {
            n.Body.Visit(this);
        }

 
        public void WriteAssembly()
        {
            _asm.Save(_assemblyName + ".exe");
        }
                
        private void SetupOperands(ASTBinary n)
        {
            //push L
            n.Left.Visit(this);

            //push R
            n.Right.Visit(this);
        }

        private void StoreLocal(string id, int index)
        {
            if (_locals.ContainsKey(id))
                _locals[id] = index;
            else
                _locals.Add(id, index);

            //store local here
            _gen.Emit(OpCodes.Stloc, _locals[id]);
        }

        private void LoadLocal(string id)
        {
            if (_locals.ContainsKey(id))
            {
                _gen.Emit(OpCodes.Ldloc, _locals[id]);
                _lastWalkedIdentifier = id;
            }
            else //??
                _lastWalkedIdentifier = "";
        }

        private Type GetCilType (ASTType n)
        {
            if (n is ASTTypeInt)
                return typeof(int);
            if (n is ASTTypeReal)
                return typeof(double);
            if (n is ASTTypeString)
                return typeof(string);
            if (n is ASTTypeBool)
                return typeof(bool);
            if (n is ASTTypeVoid)
                return typeof(void);
            if (n is ASTTypeArray)
                return GetCilType(((ASTTypeArray)n).BaseType);
            if (n is ASTTypeClass)
                return _mgr.CFlatTypes[((ASTTypeClass)n).Name].GetType();
            else
                return n.GetType();
        }
        private Type GetCilType (CFlatType t)
        {
            if (t is TypeInt)
                return typeof(int);
            if (t is TypeString)
                return typeof(string);
            if (t is TypeBool)
                return typeof(bool);
            if (t is TypeReal)
                return typeof(double);
            if (t is TypeVoid)
                return typeof(void);
            if (t is TypeArray)
                return GetCilType(((TypeArray)t).BaseType);
            else
                return t.GetType();
        }
    }
}
