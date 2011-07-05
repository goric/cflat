using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace SemanticAnalysis
{
    public class TypeReal : CFlatType
    {
        /// <summary>
        /// Not sure what the parent type should be, just going with base type class
        /// </summary>
        public override int Size
        {
            get { return 16; }
        }

        public override bool IsNumeric
        {
            get { return true; }
        }

        public override string ToString() { return "real"; }

        public override bool IsSupertype(TypeReal checkType)
        {
            return true;
        }

        public override Type CilType
        {
            get { return typeof(double); }
        }

        public override bool NeedsBoxing
        {
            get { return true; }
        }

        public override OpCode LoadElementOpCode()
        {
            return OpCodes.Ldelem_R4;
        }
    }
}
