using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
