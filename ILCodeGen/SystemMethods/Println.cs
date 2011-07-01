using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class Println : SystemMethod
    {
        public Println()
            : base("println", new TypeVoid(), new Dictionary<string, CFlatType> { { "value", new TypeObject() } })
        {

        }

        public override void Emit(ILGenerator gen)
        {
            gen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine",
                BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(object) }, null));
        }
    }
}
