using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTDereferenceArray : ASTExpression
    {
        public ASTExpression Array {get;set;}
        public ASTExpression Index { get; set; }

        public ASTDereferenceArray(ASTExpression array, ASTExpression index)
        {
            Array = array;
            Index = index;
        }

        public override String Print (int depth)
        {
            return Array.Print(depth) + "[" + Index.Print(depth) + "]";
        }

        public override void Visit (Visitor v)
        {
            v.VisitDerefArray(this);
        }
    }
}
