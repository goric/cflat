﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;

namespace SemanticAnalysis
{
    public class TypeInt : CFlatType
    {
        public override bool IsSupertype(TypeInt t) 
        {
            return true;
        }

        public override bool IsSupertype(TypeReal checkType)
        {
            return true;
        }

        public override bool IsNumeric
        {
            get { return true; }
        }

        public override string ToString() { return "int"; }

        public override int Size { get { return 4; } }

        public override Type CilType
        {
            get { return typeof(int); }
        }

        public override bool IsConcatenatable
        {
            get { return true; }
        }

        public override bool NeedsBoxing
        {
            get { return true; }
        }

        public override OpCode LoadElementOpCode
        {
            get { return OpCodes.Ldelem_I4; }
        }

        public override OpCode StoreElementOpCode
        {
            get { return OpCodes.Stelem_I4; }
        }
    }
}
