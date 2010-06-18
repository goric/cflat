using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeBool : CFlatType
    {
        public override bool IsSupertype(TypeInt t) { return false; }

        public override bool IsSupertype(TypeBool t) { return true; }

        public override String ToString() { return "bool"; }

        public override int Size { get { return 1; } }

        public bool IsSubtypeOf(CFlatType t)
        {
            return t.IsSupertype(this);
        }
    }
}
