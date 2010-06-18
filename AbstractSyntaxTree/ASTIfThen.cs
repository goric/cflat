using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTIfThen : ASTStatement
    {
        protected ASTExpression Condition { get; set; }
        protected ASTStatement Then { get; set; }

        public ASTIfThen (ASTExpression condition, ASTStatement then)
        {
            Condition = condition;
            Then = then;
        }
        
        public override String Print(int depth)
        {
            return "if " + Condition.Print(depth) + NewLine(depth) + "then " + NewLine(depth + 1) + Then.Print(depth + 1);
        }

        public override void Visit (Visitor v)
        {
            v.VisitIfThen(this);
        }
    }
}
