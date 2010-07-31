using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTStatement : ASTNode
    {
        public ASTStatement ()
        {
        }

        public override void Visit (Visitor v)
        {
            v.VisitStatement(this);
        }

        /// <summary>
        /// Takes a statement, and if it's not already a block, wraps it in an ASTBlock.
        /// </summary>
        /// <returns></returns>
        public ASTBlock WrapInBlock()
        {
            if (this is ASTBlock)
                return (ASTBlock)this;
            else
                return new ASTBlock(this);
        }
    }
}
