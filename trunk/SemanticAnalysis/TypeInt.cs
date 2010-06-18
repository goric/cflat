using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeInt : CFlatType
    {
        public override bool IsSupertype(TypeInt t) { return true; }
       
        public override bool IsSupertype(TypeBool t) { return true; }
        
        public override string ToString() { return "int"; }

        public override int Size { get { return 4; } }
       
        public bool IsSubtypeOf(CFlatType t)
        {
            return t.IsSupertype(this);
        }
    }
}
