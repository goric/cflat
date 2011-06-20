using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    /// <summary>
    /// Provides descriptors for all built-in functions for CFlat.  These are added to a top-level
    /// scope at the beginning of the second semantic pass, and are mapped to specific .NET functions
    /// in the code generation.
    /// </summary>
    public static class CFlatMethods
    {
        private class Method
        {
            public string Name { get; set; }
            public TypeFunction FuncInfo { get; set; }

            public Method (string name, TypeFunction func)
            {
                Name = name;
                FuncInfo = func;
            }
        }
        private static List<Method> _methods = new List<Method>();

        static CFlatMethods ()
        {
            _methods.Add(
                new Method("print"
                    , new TypeFunction("print")
                    {
                        ReturnType = new TypeVoid(),
                        Formals = new Dictionary<string, CFlatType> { { "value", new TypeObject() } }
                    }));

            _methods.Add(
                new Method("println"
                    , new TypeFunction("println")
                    {
                        ReturnType = new TypeVoid(),
                        Formals = new Dictionary<string, CFlatType> { { "value", new TypeObject() } }
                    }));

            _methods.Add(
               new Method("readln"
                   , new TypeFunction("readln")
                   {
                       ReturnType = new TypeString(),
                       Formals = new Dictionary<string, CFlatType> { }
                   }));

            _methods.Add(
                new Method("parseInt", new TypeFunction("parseInt")
                {
                    ReturnType = new TypeInt(),
                    Formals = new Dictionary<string,CFlatType> { { "value", new TypeString() } }
                }));

            _methods.Add(
                new Method("readFile", new TypeFunction("readFile")
                {
                    ReturnType = new TypeArray(new TypeString()),
                    Formals = new Dictionary<string, CFlatType> { { "path", new TypeString() } }
                }));
        }

        public static void AddToScope(ScopeManager mgr)
        {
            var typeClass = new TypeClass("__global");
            typeClass.Descriptor = new ClassDescriptor(typeClass, null, typeClass.ClassName, mgr.TopScope);

            foreach (var m in _methods)
            {
                m.FuncInfo.Scope = mgr.TopScope;
                mgr.AddMethod(m.Name, m.FuncInfo, typeClass, null, true);
            }
        }
    }
}
