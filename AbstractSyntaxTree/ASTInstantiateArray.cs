using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTInstantiateArray : ASTExpression
    {
        public new ASTType Type { get; set; }
        public ASTExpression Lower { get; set; }
        public ASTExpression Upper { get; set; }

        public ASTInstantiateArray (ASTType type, ASTExpression low, ASTExpression up)
        {
            Type = type;
            Lower = low;
            Upper = up;
        }

        public override String Print (int depth)
        {
            return " new " + Type.Print(depth) + "[" + Lower.Print(depth) + ".." + Upper.Print(depth) + "]";
        }

        public override void Visit (Visitor v)
        {
            v.VisitInstantiateArray(this);
        }
    }
}
