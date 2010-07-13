using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using System.Reflection.Emit;
using System.Reflection;


namespace ILCodeGen
{
    /// <summary>
    /// This class generates CIL by walking a syntax tree.  This assumes that semantic checking has already
    /// been done.  Single Pass for now.
    /// </summary>
    public class CodeGenerator : Visitor
    {
        private string _assemblyName;
        private TypeBuilder _currentType;
        private AssemblyBuilder _asm;
        private ModuleBuilder _mod;
        private ILGenerator _gen;
        private Type _lastWalkedType;
        private Dictionary<string, int> _locals;


        private MethodAttributes _tmpAttr;

        private const string MAIN_METHOD_NAME = "main";

        public CodeGenerator(string assemblyName)
            : base()
        {
            _assemblyName = assemblyName;
            _locals = new Dictionary<string, int>();
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
            
        }

        public void Generate(ASTNode n)
        {
            n.Visit(this);   
        }

        #region Declare/Define
        public override void VisitClassDefinition(ASTClassDefinition n)
        {       
            //gen type
            _currentType = _mod.DefineType(n.Name, TypeAttributes.Class | TypeAttributes.Public);
            
            n.Declarations.Visit(this);
            
            _currentType.CreateType();
                               
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

            MethodBuilder meth = _currentType.DefineMethod(n.Name, _tmpAttr);
            //set il generator to this method
            _gen = meth.GetILGenerator();
            
            //is this the entry point
            if (n.Name == MAIN_METHOD_NAME)
            {
                _asm.SetEntryPoint(meth);
            }
            
            //reset attrs
            _tmpAttr = 0;
            
            n.Body.Visit(this);

            //bail
            _gen.Emit(OpCodes.Ret);
        }

        public override void VisitDeclLocal(ASTDeclarationLocal n)
        {
            //Get type
            n.Type.Visit(this);

            LocalBuilder lb = _gen.DeclareLocal(_lastWalkedType);

            if (_locals.ContainsKey(n.ID))
                _locals[n.ID] = lb.LocalIndex;
            else
                _locals.Add(n.ID, lb.LocalIndex);

            n.InitialValue.Visit(this);

            //store local here
            _gen.Emit(OpCodes.Stloc, _locals[n.ID]);

        }
        #endregion

        #region control/invoke
        public override void VisitInvoke(ASTInvoke n)
        {
            //only console for now
        //    _gen.Emit(OpCodes.Call, typeof(Console).GetMethod(n.Method, BindingFlags.Public | BindingFlags.Static,
        //        null, new Type[] { typeof(object) }, null));
            //so... different types go on different stacks?
            n.Actuals.Visit(this);

            _gen.Emit(OpCodes.Call, typeof(Console).GetMethod(n.Method, BindingFlags.Public | BindingFlags.Static,
                null, new Type[] { typeof(int) }, null));
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

            //check condition
            n.Condition.Visit(this);

            //go to else on false
            _gen.Emit(OpCodes.Brfalse, elseLabel);

            //walk if
            n.Then.Visit(this);

            //transfer control here, then walk else body
            _gen.MarkLabel(elseLabel);
            //walk else statments
            n.Else.Visit(this);
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
            //not sure this is the right way to do this, but it works now for simple examples
            _lastWalkedType = typeof(System.Int32);
        }
        #endregion

        #region constants/primitives
        public override void VisitInteger(ASTInteger n)
        {
            //load integer
            _gen.Emit(OpCodes.Ldc_I4, n.Value);
        }
        public override void VisitString(ASTString n)
        {
            //push string on stack
            _gen.Emit(OpCodes.Ldstr, n.Value);
        }
        public override void VisitBoolean(ASTBoolean n)
        {
            //assume 1 == true?
            if (n.Val) _gen.Emit(OpCodes.Ldc_I4_1); else _gen.Emit(OpCodes.Ldc_I4_0);
        }
        public override void VisitReal(ASTReal n)
        {
            //push real on stack
            _gen.Emit(OpCodes.Ldc_R4, n.Value);
        }
        #endregion

        #region deref
        public override void VisitIdentifier(ASTIdentifier n)
        {
            if (_locals.ContainsKey(n.ID))
                _gen.Emit(OpCodes.Ldloc, _locals[n.ID]);
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

            //what to do here?  Opcodes.And is bitwise, should have 2 bool at top of stack
        }
        public override void VisitOr(ASTOr n)
        {
            SetupOperands(n);
        }

        
        #endregion


        #region unary operators
        public override void VisitNeg(ASTNegative n)
        {
           
        }

        public override void VisitNot(ASTNot n)
        {
            //evaluate expression
            n.Expression.Visit(this);

            //negate it
            _gen.Emit(OpCodes.Not);
        }

        #endregion

        public override void VisitAssign(ASTAssign n)
        {
            n.Expr.Visit(this);

            n.LValue.Visit(this);

        }
        
        public override void VisitStatementExpr(ASTStatementExpr n)
        {
            n.Expression.Visit(this);
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

        

        
    }
}
