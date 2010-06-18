using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTDeclaration : ASTNode
    {
        public ASTDeclaration() { }

        public override void Visit (Visitor v)
        {
            v.VisitDecl(this);
        }
    }
}
