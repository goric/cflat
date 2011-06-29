using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SemanticAnalysis;
using System.Reflection.Emit;
using System.Reflection;

namespace ILCodeGen.SystemMethods
{
    public abstract class SystemMethod
    {
        public SystemMethod(string name, TypeFunction info)
        {
            Name = name;
            FuncInfo = info;
        }

        public SystemMethod(string name, CFlatType returnType, Dictionary<string, CFlatType> formals)
        {
            Name = name;
            FuncInfo = new TypeFunction(name)
            {
                ReturnType = returnType,
                Formals = formals
            };
        }

        public string Name { get; protected set; }

        public TypeFunction FuncInfo { get; protected set; }

        public bool IsVoid()
        {
            return this.FuncInfo.ReturnType is TypeVoid;
        }

        public abstract void Emit(ILGenerator gen, IEnumerable<Type> argumentTypes);        
    }
}
