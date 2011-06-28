using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen.SystemMethods
{
    public class ReadFile : SystemMethod
    {
        public ReadFile()
            : base("readFile", new TypeArray(new TypeString()), new Dictionary<string, CFlatType> { { "path", new TypeString() } })
        {

        }

        public override void Emit(ILGenerator gen, IEnumerable<Type> argumentTypes)
        {
            gen.Emit(OpCodes.Call, typeof(File).GetMethod("ReadAllLines", BindingFlags.Public | BindingFlags.Static,
                null, argumentTypes.ToArray(), null));
        }
    }
}
