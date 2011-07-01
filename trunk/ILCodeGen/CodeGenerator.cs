using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using System.Reflection.Emit;
using System.Reflection;
using SemanticAnalysis;
using ILCodeGen.SystemMethods;

namespace ILCodeGen
{
    /// <summary>
    /// This class generates CIL by walking a syntax tree.  This assumes that semantic checking has already
    /// been done.  
    /// </summary>
    public class CodeGenerator : Visitor
    {
        protected string _assemblyName;

        //used for constant time lookup for the current class/method we're working on
        protected TypeBuilderInfo _currentTypeBuilder;
        protected BuilderInfo _currentMethodBuilder;

        protected AssemblyBuilder _assemblyBuilder;
        protected ModuleBuilder _moduleBuilder;
        //each MethodBuilder or ConstructorBuilder will reuse this instance for generating IL
        protected ILGenerator _gen;

        protected Type _lastWalkedType;

        /* all of these are getting replaced. */
        
        protected string _lastWalkedIdentifier;
        protected Dictionary<string, int> _locals;
        protected TypeManager _typeManager;
        private Stack<Type> _typesOnStack;
        private bool _isArrayAssign;
        private MethodAttributes _tmpAttr;

        public static string MainMethodName
        {
            get { return "main"; }
        }

        public CodeGenerator(string assemblyName)
            : base()
        {
            _assemblyName = assemblyName;
            _locals = new Dictionary<string, int>();
            _typesOnStack = new Stack<Type>();

            InitAssembly();
        }

        /// <summary>
        /// Sets up the resulting executable, AssemblyBuilder, and ModuleBuilder
        /// </summary>
        private void InitAssembly()
        {
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName(_assemblyName),
                AssemblyBuilderAccess.RunAndSave);
            //Create Module
            string exeName = _assemblyName + ".exe";
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(exeName, exeName);

            _typeManager = new TypeManager(_moduleBuilder);
        }

        public void Generate(ASTNode n)
        {
            //run two initial passes to collect classes, and all definitions
            n.Visit(new ClassPass(_typeManager));
            n.Visit(new DeclarationPass(_typeManager));
            //run another pass to fill in all the details of methods
            n.Visit(this);
            _typeManager.CreateAllTypes();            
        }

        #region Declare/Define

        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            _currentTypeBuilder = _typeManager.GetBuilderInfo(n.Name);
            n.Declarations.Visit(this);                 
        }

        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            throw new NotImplementedException("Inheritance is not yet finished");

            //_currentTypeBuilder = _typeManager.GetBuilder(n.Name);
            //n.Declarations.Visit(this);
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

        public override void VisitDeclMethod(ASTDeclarationMethod n)
        {
            n.Modifiers.Visit(this);

            MethodBuilderInfo methodInfo = _typeManager.GetMethodBuilderInfo(_currentTypeBuilder.Name, n.Name);
            //store a reference so we can access locals, arguments, and stuff about the current method we're working on.
            _currentMethodBuilder = methodInfo;
            _gen = methodInfo.Builder.GetILGenerator();
            //is this the entry point
            if (n.Name == MainMethodName)
            {
                _assemblyBuilder.SetEntryPoint(methodInfo.Builder);
            }

            n.Body.Visit(this);
            //we need to explicitly put a return statement for voids
            if (_typeManager.LookupCilType(n.ReturnType) == typeof(void))
                _gen.Emit(OpCodes.Ret);
        }

        public override void VisitDeclLocal(ASTDeclarationLocal n)
        {
            //Get type
            n.Type.Visit(this);
            LocalBuilder builder = _gen.DeclareLocal(_lastWalkedType);
            //keep track of the LocalBuilder
            LocalBuilderInfo info = _currentMethodBuilder.AddLocal(n.ID, builder);
            //set value if needed
            if (n.InitialValue != null)
            {
                n.InitialValue.Visit(this);
                //store it to the local
                _gen.Emit(OpCodes.Stloc, info.Index);
            }
        }

        public override void VisitDeclField(ASTDeclarationField n)
        {
            //nothing to do here, already done in the DeclarationPass
        }

        public override void VisitDeclConstructor(ASTDeclarationCtor n)
        {
            //lookup base class for the current type builder
            if (_currentTypeBuilder.Builder.BaseType is object)
            {
                _currentMethodBuilder = _currentTypeBuilder.ConstructorBuilder;
                _gen = _currentTypeBuilder.ConstructorBuilder.Builder.GetILGenerator();
                //invoke the constructor of object
                ConstructorInfo baseCtor = typeof(object).GetConstructor(Type.EmptyTypes);
                _gen.Emit(OpCodes.Ldarg_0); //this.
                _gen.Emit(OpCodes.Call, baseCtor);
                //visit the body of the constructor
                n.Body.Visit(this);
                _gen.Emit(OpCodes.Ret);
            }
            else
                throw new NotImplementedException("Inheritance is totally not done yet.");
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
            var info = _typeManager.GetBuilderInfo(n.ClassName);
            _gen.Emit(OpCodes.Newobj, info.ConstructorBuilder.Builder);
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
            if (SystemMethodManager.IsSystemMethod(n.Method))
            {
                n.Actuals.Visit(this);
                SystemMethod method = SystemMethodManager.Lookup(n.Method);
                method.Emit(_gen);
            }
            else
            {
                //push who
                n.Object.Visit(this);
                Type who = _lastWalkedType;
                //push actuals
                n.Actuals.Visit(this);
                //find the method to execute on the given class
                MethodBuilderInfo info = _typeManager.GetMethodBuilderInfo(who.Name, n.Method);
                _gen.Emit(OpCodes.Callvirt, info.Builder);
            }
        }

        public override void VisitBase(ASTBase n)
        {
            throw new NotImplementedException();
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

            //load value into temp variable - set type so _typeManager.LookupCilType()
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
            _lastWalkedType = _typeManager.LookupCilType(n);
        }

        public override void VisitTypeBool(ASTTypeBool n)
        {
            _lastWalkedType = _typeManager.LookupCilType(n);
        }

        public override void VisitTypeReal(ASTTypeReal n)
        {
            _lastWalkedType = _typeManager.LookupCilType(n);
        }

        public override void VisitTypeString(ASTTypeString n)
        {
            _lastWalkedType = _typeManager.LookupCilType(n);
        }

        public override void VisitTypeVoid(ASTTypeVoid n)
        {
            _lastWalkedType = _typeManager.LookupCilType(n);
        }

        public override void VisitTypeName(ASTTypeClass n)
        {
            _lastWalkedType = _typeManager.LookupCilType(n);
        }

        public override void VisitTypeArray (ASTTypeArray n)
        {
            _lastWalkedType = _typeManager.LookupCilType(n);
        }

        public override void VisitTypeChar(ASTTypeChar n)
        {
            _lastWalkedType = _typeManager.LookupCilType(n);
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
            //push string on stack, we need to trim off the two "s that our grammar adds
            string escaped = n.Value.Substring(1, n.Value.Length - 2); 
            _gen.Emit(OpCodes.Ldstr, escaped);
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
        public override void VisitChar(ASTChar n)
        {
            _gen.Emit(OpCodes.Ldc_I4_S, n.Val);
            _typesOnStack.Push(typeof(char));
            _lastWalkedType = typeof(char);
        }

        #endregion

        #region deref

        public override void VisitDerefArray (ASTDereferenceArray n)
        {
            LoadLocal(((ASTIdentifier)n.Array).ID);
            n.Index.Visit(this);
            var type = _typeManager.LookupCilType(n.CFlatType);
            _typesOnStack.Push(type);
            _lastWalkedType = type;
            //bit of a hack, we need to gen a ldelem on derefences except when its an assignment
            if (!_isArrayAssign)
            {
                if (_lastWalkedType == typeof(int))
                    _gen.Emit(OpCodes.Ldelem_I4);
                if (_lastWalkedType == typeof(double))
                    _gen.Emit(OpCodes.Ldelem_R4);
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
            BoxIfNeeded(n.Left.CFlatType);
            n.Right.Visit(this);
            BoxIfNeeded(n.Right.CFlatType);
            
            //now call String.Concat
            _gen.Emit(OpCodes.Call, typeof(String).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static,
               null, new Type[] { typeof(object), typeof(object) }, null));
        }

        private void BoxIfNeeded(CFlatType t)
        {
            if (t.NeedsBoxing)
                _gen.Emit(OpCodes.Box, t.CilType);
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

        public override void VisitIdentifier(ASTIdentifier n)
        {
            if (n.IsLeftHandSide)
            {

            }
            else
            {
                if (IsIdentifierLocal(n.ID))
                {
                    var info = _currentMethodBuilder.Locals[n.ID];
                    _gen.Emit(OpCodes.Ldloc, info.Index);

                    _lastWalkedType = info.Builder.LocalType;
                }
                else if (IsIdentifierField(n.ID))
                {
                    FieldBuilder field = _currentTypeBuilder.FieldMap[n.ID];
                    _gen.Emit(OpCodes.Ldarg_0);
                    _gen.Emit(OpCodes.Ldfld, field);

                    _lastWalkedType = field.FieldType;
                }
                else if (IsIdentifierArgument(n.ID))
                {
                    ArgumentInfo info = _currentMethodBuilder.Arguments[n.ID];
                    int index = info.Index;
                    if (!_currentMethodBuilder.Method.IsStatic)
                        index++;
                    _gen.Emit(OpCodes.Ldarg, index);

                    _lastWalkedType = info.CilType;
                }
            }
        }

        public override void VisitSelf(ASTSelf n)
        {
            _lastWalkedType = _currentTypeBuilder.Builder;
            _gen.Emit(OpCodes.Ldarg_0);
        }

        public override void VisitAssign(ASTAssign n)
        {
            //for arrays we need to visit the left side first, then the expression,
            // making sure not to emit a ldelem for the LHS array dereference,
            // and also emit a stelem command
            if (n.LValue is ASTDereferenceArray)
            {
                throw new NotImplementedException("Assigning to an index of an array has yet to be refactored.");
                /*_isArrayAssign = true;
                n.LValue.Visit(this);
                _isArrayAssign = false;
                n.Expr.Visit(this);
                _gen.Emit(OpCodes.Stelem_I4);
                return;*/
            }
            else if (n.LValue is ASTIdentifier)
            {
                ASTIdentifier lvalue = (ASTIdentifier)n.LValue;
                if (IsIdentifierLocal(lvalue.ID))
                {
                    LocalBuilderInfo localInfo = _currentMethodBuilder.Locals[lvalue.ID];
                    //visit the expression, which will put the value on the stack
                    n.Expr.Visit(this);
                    //assign the local
                    _gen.Emit(OpCodes.Stloc, localInfo.Index);
                }
                else if (IsIdentifierField(lvalue.ID))
                {
                    FieldBuilder field = _currentTypeBuilder.FieldMap[lvalue.ID];
                    //push the "this" argument
                    _gen.Emit(OpCodes.Ldarg_0);
                    //visit the expression, which will put the return value on the stack
                    n.Expr.Visit(this);
                    _gen.Emit(OpCodes.Stfld, field);
                }
                else if (IsIdentifierArgument(lvalue.ID))
                {
                    //visit the expression
                    n.Expr.Visit(this);
                    //store it at the argument index
                    _gen.Emit(OpCodes.Starg, _currentMethodBuilder.Arguments[lvalue.ID].Index);
                }
                else
                    throw new Exception(String.Format("Identifier '{0}' is not a local, argument, or member variable of the current class.", lvalue.ID));
            }
            else if (n.LValue is ASTDereferenceField)
            {
                ASTDereferenceField lvalue = (ASTDereferenceField)n.LValue;
                lvalue.Object.Visit(this);
                Type who = _lastWalkedType;
                TypeBuilderInfo info = _typeManager.GetBuilderInfo(who.Name);
                FieldBuilder field = info.FieldMap[lvalue.Field];
                //visit right hand side
                n.Expr.Visit(this);
                _gen.Emit(OpCodes.Stfld, field);
            }
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
            _assemblyBuilder.Save(_assemblyName + ".exe");
        }
                
        private void SetupOperands(ASTBinary n)
        {
            //push L
            n.Left.Visit(this);

            //push R
            n.Right.Visit(this);
        }

        private bool IsIdentifierLocal(string name)
        {
            return _currentMethodBuilder.Locals.ContainsKey(name);
        }

        private bool IsIdentifierField(string name)
        {
            return _currentTypeBuilder.FieldMap.ContainsKey(name);
        }

        private bool IsIdentifierArgument(string name)
        {
            return _currentMethodBuilder.Arguments.ContainsKey(name);
        }

        [Obsolete("Shit's gettin' refactored!")]
        private void StoreLocal(string id, int index)
        {
            if (_locals.ContainsKey(id))
                _locals[id] = index;
            else
                _locals.Add(id, index);

            //store local here
            _gen.Emit(OpCodes.Stloc, _locals[id]);
        }

        [Obsolete("Shit's gettin' refactored!")]
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
    }
}
