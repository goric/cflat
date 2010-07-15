using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeInt : CFlatType
    {
        public override bool IsSupertype(TypeInt t) { return true; }
       
        //I don't think a bool should be a super type, unless we're going C++ style 
        //public override bool IsSupertype(TypeBool t) { return true; }
        
        public override string ToString() { return "int"; }

        public override int Size { get { return 4; } }
    }
}
