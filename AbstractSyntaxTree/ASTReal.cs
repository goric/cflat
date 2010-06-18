using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTReal : ASTExpression
    {
        public double Value { get; set; }

        public ASTReal (double val)
        {
            Value = val;
        }
        
        public override String Print(int depth)
        {
            return Value.ToString();
        }

        public override void Visit (Visitor v)
        {
            v.VisitReal(this);
        }
    }
}
