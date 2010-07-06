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
    /// MES - just screwed this up, but don't want to lose what i have
    /// checking in, though broken.   it does compile...
    /// </summary>
    public class CodeGenerator : Visitor
    {
        private string _assemblyName;
        private TypeBuilder _currentType;
        private AssemblyBuilder _asm;
        private ModuleBuilder _mod;
        private ILGenerator _gen;


        private MethodAttributes _tmpAttr;

        private const string MAIN_METHOD_NAME = "main";

        public CodeGenerator(string assemblyName)
            : base()
        {
            _assemblyName = assemblyName;
            
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

        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            //gen type            
            _currentType = _mod.DefineType(n.Name, TypeAttributes.Class | TypeAttributes.Public);
            
            //gen/find main method - need to update for access modifiers
            //MethodBuilder main = _currentType.DefineMethod("Main",
            //MethodAttributes.Public | MethodAttributes.Static);
            //start CIL generation
            //ILGenerator il = main.GetILGenerator();
            //il.Emit(OpCodes.Ldstr, "Hi, CSCI6480");
            //il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static,
            //    null, new Type[] { typeof(String) }, null));
            //il.Emit(OpCodes.Ret);
            
            _currentType.CreateType();
                                   
            //_asm.SetEntryPoint(main);

        }


        public override void VisitModifierList(ASTModifierList n)
        {
            //bitwise OR what we have with the next modifier in the list
            _tmpAttr = _tmpAttr | System.Reflection.MethodAttributes.Public;
        }

        public override void VisitDeclMethod(ASTDeclarationMethod n)
        {

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
        }

        public override void VisitInteger(ASTInteger n)
        {
            //load integer
            _gen.Emit(OpCodes.Ldc_I4, n.Value);
        }

        public override void VisitAdd(ASTAdd n)
        {
            //saw left alrady, should be on stack, push right on stack now?
            
            //pop 2 numbers, add, push result
            _gen.Emit(OpCodes.Add);

        }

        public void WriteAssembly()
        {
            
            _asm.Save(_assemblyName + ".exe");
        }

        
    }
}
