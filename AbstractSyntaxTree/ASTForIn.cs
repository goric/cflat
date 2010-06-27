using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTForIn : ASTStatement
    {
        public ASTIdentifier TempVariable { get; set; }
        public ASTExpression Lower { get; set; }
        public ASTExpression Upper { get; set; }
        public ASTStatement Body { get; set; }

        public ASTForIn (ASTIdentifier variable, ASTExpression lower, ASTExpression upper, ASTStatement body)
        {
            TempVariable = variable;
            Lower = lower;
            Upper = upper;
            Body = body;
        }

        public override string Print (int depth)
        {
            return "for (" + TempVariable.Print(depth) + " in [" + Lower.Print(depth) + " .. " + Upper.Print(depth) + "])"
                + NewLine(depth + 1) + Body.Print(depth + 1);
        }

        public override void Visit (Visitor v)
        {
            v.VisitForIn(this);
        }
    }
}