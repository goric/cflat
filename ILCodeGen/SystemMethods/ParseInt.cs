using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class ParseInt : SystemMethod
    {
        public ParseInt()
            : base("parseInt", new TypeInt(), new Dictionary<string, CFlatType> { { "value", new TypeString() } })
        {

        }

        public override void Emit(ILGenerator gen)
        {
            gen.Emit(OpCodes.Call, typeof(Int32).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static,
                null, new Type[] { typeof(string) }, null));
        }
    }
}
