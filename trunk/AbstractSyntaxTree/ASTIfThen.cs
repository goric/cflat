using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTIfThen : ASTStatement
    {
        public ASTExpression Condition { get; set; }
        public ASTBlock Then { get; set; }

        public ASTIfThen (ASTExpression condition, ASTStatement then)
        {
            Condition = condition;
            //I think it's most appropriate to record the presence of the implied block around 1 line statements here, rather than in the grammar.
            if (then is ASTBlock)
                Then = (ASTBlock)then;
            else
                Then = new ASTBlock(then);

            Then.IsBranch = true;
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
