using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTNoop : ASTStatement
    {
        public ASTNoop ()
        {
        }

        public override String Print (int depth)
        {
            return "noop";
        }

        public override void Visit (Visitor v)
        {
            v.VisitNoop(this);
        }
    }
}
