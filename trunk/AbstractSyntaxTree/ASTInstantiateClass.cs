using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTInstantiateClass : ASTExpression
    {
        public String ClassName { get; set; }
        public ASTExpressionList Actuals { get; set; }
        public Descriptor Descriptor { get; set; }
        public Descriptor ClassDescriptor { get; set; }

        public ASTInstantiateClass (String className, ASTExpressionList actuals)
        {
            ClassName = className;
            Actuals = actuals;
        }

        public override String Print (int depth)
        {
            return " new " + ClassName + "(" + Actuals.Print(depth) + ")";
        }

        public override void Visit (Visitor v)
        {
            v.VisitInstantiateClass(this);
        }
    }
}
