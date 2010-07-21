using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTDereferenceField : ASTLValue
    {
        public ASTExpression Object { get; set; }
        public String Field { get; set; }

        public ASTDereferenceField (ASTExpression obj, String field)
        {
            Object = obj;
            Field = field;
        }
        
        public override String Print(int depth)
        {
            return Object.Print(depth) + "." + Field;// + " : " + _tty.toString();
        }

        public override void Visit (Visitor v)
        {
            v.VisitDerefField(this);
        }
    }
}
