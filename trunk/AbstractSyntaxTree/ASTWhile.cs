using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTWhile : ASTStatement
    {
        public ASTExpression Condition { get; set; }
        public ASTStatement Body { get; set; }

        public ASTWhile (ASTExpression condition, ASTStatement body)
        {
            Condition = condition;
            Body = body;
        }
        
        public override String Print(int depth)
        {
            return "while (" + Condition.Print(depth) + ") do " + NewLine(depth + 1) + Body.Print(depth + 1);
        }

        public override void Visit (Visitor v)
        {
            v.VisitWhile(this);
        }
    }
}
