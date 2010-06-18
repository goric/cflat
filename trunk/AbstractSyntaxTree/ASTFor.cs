using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTFor : ASTStatement
    {
        public ASTStatement InitialExpr { get; set; }
        public ASTExpression Conditional { get; set; }
        public ASTExpression LoopExpr { get; set; }
        public ASTStatement Body { get; set; }

        public ASTFor(ASTStatement init, ASTExpression conditional, ASTExpression loop, ASTStatement body)
        {
            InitialExpr = init;
            Conditional = conditional;
            LoopExpr = loop;
            Body = body;
        }

        public override string Print (int depth)
        {
            return "for (" + InitialExpr.Print(depth) + ";" + Conditional.Print(depth) + ";" + LoopExpr.Print(depth) + ")"
                        + NewLine(depth+1) + Body.Print(depth+1);
        }

        public override void Visit (Visitor v)
        {
            v.VisitFor(this);
        }
    }
}
