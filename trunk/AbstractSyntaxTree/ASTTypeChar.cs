using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTTypeChar : ASTType
    {
        public ASTTypeChar() { }

        public override String Print(int depth)
        {
            return "char";
        }

        public override void Visit(Visitor v)
        {
            v.VisitTypeChar(this);
        }
    }
}
