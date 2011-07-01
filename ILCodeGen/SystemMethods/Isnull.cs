using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class Isnull : SystemMethod
    {
        public Isnull()
            : base("isnull", new TypeBool(), new Dictionary<string, CFlatType> { { "value", new TypeObject() } })
        {

        }

        public override void Emit(ILGenerator gen)
        {
            gen.Emit(OpCodes.Ldnull);
            gen.Emit(OpCodes.Ceq);
        }
    }
}
