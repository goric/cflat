using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTInvoke : ASTExpression
    {
        public ASTExpression Object { get; set; }
        public String Method { get; set; }
        public ASTExpressionList Actuals { get; set; }
        public Descriptor Descriptor { get; set; }
        
        public ASTInvoke(ASTExpression obj, String method, ASTExpressionList actuals)
        {
            Object = obj;
            Method = method;
            Actuals = actuals;
        }

        public override String Print(int depth)
        {
            return Object.Print(depth) + "." + Method + "(" + Actuals.Print(depth) + ")";
        }

        public override void Visit (Visitor v)
        {
            v.VisitInvoke(this);
        }
    }
}
