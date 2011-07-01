using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class Print : SystemMethod
    {
        public Print()
            : base("print", new TypeVoid(), new Dictionary<string, CFlatType> { { "value", new TypeObject() } })
        {

        }

        public override void Emit(ILGenerator gen)
        {
            gen.Emit(OpCodes.Call, typeof(Console).GetMethod("Write",
                BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(object) }, null));
        }
    }
}
