using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class Die : SystemMethod
    {
        public Die()
            : base("die", new TypeVoid(), new Dictionary<string, CFlatType> { { "msg", new TypeString() } })
        {

        }

        public override void Emit(ILGenerator gen)
        {
            gen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine",
                BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null));
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Call, typeof(Environment).GetMethod("Exit"));
        }
    }
}
