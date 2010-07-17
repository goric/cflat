using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTNode
    {
        public SourceLocation Location { get; set; }
        public virtual CFlatType CFlatType { get; set; }
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

        public string CheckNullPrint(ASTNode n, int depth)
        {
            return (n != null) ? n.Print(depth) : string.Empty;
        }
    }
}
