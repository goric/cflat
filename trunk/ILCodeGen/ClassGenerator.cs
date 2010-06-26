using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using System.Reflection.Emit;
using System.Reflection;

namespace ILCodeGen
{
    public class ClassGenerator : Visitor
    {
        private string _topClass;

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
                AssemblyBuilder asm = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(_topClass), AssemblyBuilderAccess.RunAndSave);
                string exeName = _topClass + ".exe";
                ModuleBuilder mod = asm.DefineDynamicModule(exeName,
                    exeName);
                TypeBuilder type = mod.DefineType(_topClass, TypeAttributes.Class);
                //gen/find main method
                MethodBuilder main = type.DefineMethod("Main",
                MethodAttributes.Public | MethodAttributes.Static);
                //start CIL generation
                ILGenerator il = main.GetILGenerator();
                il.Emit(OpCodes.Ldstr, "Hi, CSCI6480");
                il.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", BindingFlags.Public | BindingFlags.Static,
                    null, new Type[] { typeof(String) }, null));
                il.Emit(OpCodes.Ret);
                type.CreateType();
                asm.SetEntryPoint(main);
                asm.Save(exeName);

            }
        }
    }
}
