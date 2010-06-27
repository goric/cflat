using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTReturn : ASTStatement
    {
        protected ASTExpression ReturnValue { get; set; }
        public Type Type { get; set; }

        public ASTReturn (ASTExpression retVal)
        {
            ReturnValue = retVal;
        }
        
        public override String Print(int depth)
        {
            return "return " + ReturnValue.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitReturn(this);
        }
    }
}
