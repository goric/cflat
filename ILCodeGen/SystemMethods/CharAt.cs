using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class CharAt : SystemMethod
    {
        public CharAt()
            : base("charAt", new TypeChar(), new Dictionary<string, CFlatType> { { "source", new TypeString() }, { "index", new TypeInt() } })
        {

        }

        public override void Emit(ILGenerator gen, IEnumerable<Type> argumentTypes)
        {
            gen.Emit(OpCodes.Call, typeof(String).GetMethod("get_Chars"));
        }
    }
}
