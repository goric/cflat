using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTBlock : ASTStatement
    {
        public ASTStatement Body { get; set; }

        public ASTBlock (ASTStatement body)
        {
            Body = body;
        }
        
        public override String Print(int depth)
        {
            return "{" + NewLine(depth + 1) + Body.Print(depth + 1) + NewLine(depth) + "}";
        }

        public override void Visit (Visitor v)
        {
            v.VisitBlock(this);
        }
    }
}
