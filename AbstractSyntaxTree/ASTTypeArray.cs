using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTTypeArray : ASTType
    {
        public ASTType BaseType { get; set; }

        public ASTTypeArray (ASTType baseType)
        {
            BaseType = baseType;
        }

        public override String Print (int depth)
        {
            return BaseType.Print(depth) + "[]";
        }

        public override void Visit (Visitor v)
        {
            v.VisitTypeArray(this);
        }
    }
}
