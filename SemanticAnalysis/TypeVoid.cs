using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeVoid : CFlatType
    {
        public override int Size
        {
            get { return 0; }
        }

        public override bool IsSupertype(TypeVoid checkType)
        {
            return true;
        }

        public override string ToString() { return "void"; }

        public override Type CilType
        {
            get { return typeof(void); }
        }
    }
}
