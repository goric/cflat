using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTTypeString : ASTType
    {
        public ASTTypeString ()
        {
        }

        public override String Print (int depth)
        {
            return "string";
        }

        public override void Visit (Visitor v)
        {
            v.VisitTypeString(this);
        }
    }
}
