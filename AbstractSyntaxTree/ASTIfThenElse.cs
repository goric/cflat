using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTIfThenElse : ASTIfThen
    {
        private ASTStatement Else;

        public ASTIfThenElse (ASTExpression condition, ASTStatement then, ASTStatement elseStatement)
            : base(condition, then)
        {
            Else = elseStatement;
        }
        
        public override String Print(int depth)
        {
            return base.Print(depth) + NewLine(depth) + "else " + NewLine(depth + 1) + Else.Print(depth + 1);
        }

        public override void Visit (Visitor v)
        {
            v.VisitIfThenElse(this);
        }
    }
}
