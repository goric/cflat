using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTIdentifier : ASTExpression
    {
        public SourceLocation Location { get; set; }
        public String ID { get; set; }
        public Descriptor Descriptor { get; set; }
        public Descriptor SelfDescriptor { get; set; }

        public ASTIdentifier(SourceLocation loc, String id)
        {
            Location = loc;
            ID = id;
        }
        
        public override String Print(int depth)
        {
            return ID;
        }

        public override void Visit (Visitor v)
        {
            v.VisitIdentifier(this);
        }
    }
}
