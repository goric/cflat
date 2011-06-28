using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeChar : CFlatType
    {
        public override bool IsSupertype(TypeChar t) { return true; }

        public override String ToString() { return "char"; }

        public override int Size { get { return 2; } }

        public override bool IsChar { get { return true; } }

        public override Type CilType
        {
            get { return typeof(char); }
        }

        public override bool IsConcatenatable
        {
            get { return true; }
        }

        public override bool NeedsBoxing
        {
            get { return true; }
        }
    }
}
