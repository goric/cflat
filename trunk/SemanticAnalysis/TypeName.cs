using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeName : CFlatType
    {
        public CFlatType Base {get;set;}

        public override bool IsSupertype(TypeName checkType)
        {
            return true;
        }

        public TypeName (CFlatType bse)
        {
            Base = bse;
        }
    }
}
