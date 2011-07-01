using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public abstract class ASTLValue : ASTExpression
    {
        public Descriptor Descriptor { get; set; }

        public bool IsLeftHandSide { get; set; }
    }
}
