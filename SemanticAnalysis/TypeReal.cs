using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeReal : CFlatType
    {
        /// <summary>
        /// Not sure what the parent type should be, just going with base type class
        /// </summary>
        public override int Size
        {
            get
            {
                return 16;
            }
        }

        public override string ToString() { return "real"; }

        public override bool IsSupertype(TypeInt checkType)
        {
            return true;
        }
    }
}
