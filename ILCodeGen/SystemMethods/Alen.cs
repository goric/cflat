using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class Alen : SystemMethod
    {
        public Alen()
            : base("alen", new TypeInt(), new Dictionary<string, CFlatType> { { "value", new TypeAnyArray() } })
        {

        }

        public override void Emit(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldlen);
        }
    }
}
