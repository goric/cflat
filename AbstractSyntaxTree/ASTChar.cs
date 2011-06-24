using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTChar : ASTExpression
    {
        public char Val { get; protected set; }

        public ASTChar(char value)
        {
            Val = value;
        }
        
        public override String Print(int depth)
        {
            return Val.ToString();
        }

        public override void Visit (Visitor v)
        {
            v.VisitChar(this);
        }
    }
}
