using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTTypeBool : ASTType
    {
        public ASTTypeBool ()
        {
        }

        public override String Print (int depth)
        {
            return "bool";
        }

        public override void Visit (Visitor v)
        {
            v.VisitTypeBool(this);
        }
    }
}
