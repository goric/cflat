using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTIfThen : ASTStatement
    {
        public ASTExpression Condition { get; set; }
        public ASTStatement Then { get; set; }

        public ASTIfThen (ASTExpression condition, ASTStatement then)
        {
            Condition = condition;
            Then = then;
        }
        
        public override String Print(int depth)
        {
            return "if (" + Condition.Print(depth) + ")" + NewLine(depth + 1) + Then.Print(depth + 1);
        }

        public override void Visit (Visitor v)
        {
            v.VisitIfThen(this);
        }
    }
}
