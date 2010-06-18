using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTStatement : ASTNode
    {
        public ASTStatement ()
        {
        }

        public override void Visit (Visitor v)
        {
            v.VisitStatement(this);
        }
    }
}
