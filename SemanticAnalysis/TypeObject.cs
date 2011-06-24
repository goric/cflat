using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    /// <summary>
    /// Like in C#, this is the superclass of everything.
    /// </summary>
    public class TypeObject : CFlatType
    {
        public override bool IsSupertype (TypeArray checkType) { return true; }
        public override bool IsSupertype (TypeBool checkType) { return true; }
        public override bool IsSupertype (TypeClass checkType) { return true; }
        public override bool IsSupertype (TypeFunction checkType) { return true; }
        public override bool IsSupertype (TypeInt checkType) { return true; }
        public override bool IsSupertype (TypeReal checkType) { return true; }
        public override bool IsSupertype (TypeString checkType) { return true; }
        public override bool IsSupertype (TypeVoid checkType) { return true; }

        public override Type CilType
        {
            get { return typeof(object); }
        }
    }
}
