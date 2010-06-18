using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeFunction : CFlatType
    {
        public override bool IsFunction
        {
            get
            {
                return true;
            }
        }

        public override string ToString() { return ""; }
    }
}
