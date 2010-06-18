using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTTypeInt : ASTType
    {
        public ASTTypeInt ()
        {
        }

        public override String Print (int depth)
        {
            return "int";
        }

        public override void Visit (Visitor v)
        {
            v.VisitTypeInt(this);
        }
    }
}
