using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTNode
    {
        protected String NewLine(int depth)
        {
            var s = new StringBuilder(Environment.NewLine);
            
            for (int i = 0; i < depth * 3; i++)
                s.Append(' ');

            return s.ToString();
        }

        public virtual String Print(int depth)
        {
            return "";
        }

        public virtual void Visit (Visitor v)
        {
            v.VisitNode(this);
        }
    }
}
