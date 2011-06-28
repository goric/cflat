using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTChar : ASTExpression
    {
        public char Val { get; protected set; }

        public ASTChar(string value)
        {
            //incoming string will hold '\uhhh' where h is a hex digit.
            Val = (char)Convert.ToInt32(value.Substring(3, 4), 16);
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
