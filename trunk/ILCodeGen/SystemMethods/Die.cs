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
            //This is a bit of a hack here, Environment.Exit doesn't count for return statements,
            //so I'm going to add an exception that never gets thrown. Yeah, this is lame.
            gen.Emit(OpCodes.Newobj, typeof(Exception).GetConstructor(Type.EmptyTypes));
            gen.Emit(OpCodes.Throw);
        }

        public override bool IsExitStatement
        {
            get { return true; }
        }
    }
}
