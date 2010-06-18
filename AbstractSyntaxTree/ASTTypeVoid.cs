using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTTypeVoid : ASTType
    {
        public ASTTypeVoid ()
        {
        }

        public override String Print (int depth)
        {
            return "void";
        }

        public override void Visit (Visitor v)
        {
            v.VisitTypeVoid(this);
        }
    }
}
