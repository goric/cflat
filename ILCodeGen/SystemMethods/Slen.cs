using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class Slen : SystemMethod
    {
        public Slen()
            : base("slen", new TypeInt(), new Dictionary<string, CFlatType> { { "value", new TypeString() } })
        {

        }

        public override void Emit(ILGenerator gen, IEnumerable<Type> argumentTypes)
        {
            gen.Emit(OpCodes.Call, typeof(String).GetMethod("get_Length"));
        }
    }
}
