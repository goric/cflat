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
    public class ClassGenerator : Visitor
    {
        private string _topClass;
        private TypeBuilder _currentType;
        private AssemblyBuilder _asm;
        private ModuleBuilder _mod;
        private ILGenerator _gen;

        public void Generate(ASTNode n)
        {
            n.Visit(this);   
        }

        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            //top level class - will this always be first?  may need to search for a main method first.
            if (String.IsNullOrEmpty(_topClass)) _topClass = n.Name;

           

            if (n.Name == _topClass)
            {
                //generate top-level class
                _asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(_topClass), AssemblyBuilderAccess.RunAndSave);
                    string exeName = _topClass + ".exe";
                _mod = _asm.DefineDynamicModule(exeName,
                    exeName);
            }
            
            //gen type            
            _currentType = _mod.DefineType(n.Name, TypeAttributes.Class);
            //gen/find main method - need to update for access modifiers
            MethodBuilder main = _currentType.DefineMethod("Main",
            MethodAttributes.Public | MethodAttributes.Static);
            //start CIL generation
            ILGenerator il = main.GetILGenerator();
            il.Emit(OpCodes.Ldstr, "Hi, CSCI6480");
            il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static,
                null, new Type[] { typeof(String) }, null));
            il.Emit(OpCodes.Ret);
            _currentType.CreateType();
                                   
            _asm.SetEntryPoint(main);
            

        }

        public override void VisitDeclMethod(ASTDeclarationMethod n)
        {
            //public static for now
            MethodBuilder method = _currentType.DefineMethod(n.Name, MethodAttributes.Public | MethodAttributes.Static);

            //set il generator to this method?
            _gen = method.GetILGenerator();
        }
    
        public override void VisitAdd(ASTAdd n)
        {
            //add 2 numbers
            _gen.Emit(OpCodes.Ldobj, n.Left.ToString());
            _gen.Emit(OpCodes.Ldobj, n.Right.ToString());
            _gen.Emit(OpCodes.Add);
        }


        public void WriteAssembly()
        {
            //should really be a simpler way to get the simple assembly name...
            _asm.Save(_mod.Assembly.FullName.Substring(0, _mod.Assembly.FullName.IndexOf(','));
        }
    }
}
