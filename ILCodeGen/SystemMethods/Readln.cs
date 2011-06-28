using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class Readln : SystemMethod
    {
        public Readln()
            : base("readln", new TypeString(), new Dictionary<string, CFlatType> { })
        {

        }

        public override void Emit(ILGenerator gen, IEnumerable<Type> argumentTypes)
        {
            gen.Emit(OpCodes.Call, typeof(Console).GetMethod("ReadLine", BindingFlags.Public | BindingFlags.Static,
                null, System.Type.EmptyTypes, null));
        }
    }
}
