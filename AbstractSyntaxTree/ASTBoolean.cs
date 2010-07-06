using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTBoolean : ASTExpression
    {
        public bool Val { get; protected set; }

        public ASTBoolean (bool value)
        {
            Val = value;
        }
        
        public override String Print(int depth)
        {
            return Val.ToString();
        }

        public override void Visit (Visitor v)
        {
            v.VisitBoolean(this);
        }
    }
}
