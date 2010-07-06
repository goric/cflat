using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeName : CFlatType
    {
        public CFlatType Base {get;set;}
        public TypeName (CFlatType bse)
        {
            Base = bse;
        }
    }
}
