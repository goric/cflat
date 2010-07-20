﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTFor : ASTStatement
    {
        public ASTDeclarationLocal InitialExpr { get; set; }
        public ASTExpression Conditional { get; set; }
        public ASTExpression LoopExpr { get; set; }
        public ASTStatement Body { get; set; }

        public ASTFor (ASTDeclarationLocal init, ASTExpression conditional, ASTExpression loop, ASTStatement body)
        {
            InitialExpr = init;
            Conditional = conditional;
            LoopExpr = loop;
            Body = body;
        }

        public override string Print (int depth)
        {
            return "for (" + CheckNullPrint(InitialExpr, depth) + ";" + CheckNullPrint(Conditional, depth) + ";" + CheckNullPrint(LoopExpr, depth) + ")"
                        + NewLine(depth+1) + Body.Print(depth+1);
        }

        public override void Visit (Visitor v)
        {
            v.VisitFor(this);
        }


    }
}
