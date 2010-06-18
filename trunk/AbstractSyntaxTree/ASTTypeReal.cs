using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTTypeReal : ASTType
    {
        public ASTTypeReal ()
        {
        }

        public override String Print (int depth)
        {
            return "real";
        }

        public override void Visit (Visitor v)
        {
            v.VisitTypeReal(this);
        }
    }
}
