using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;
using QUT.Gppg;

namespace AbstractSyntaxTree
{
    public class ASTIdentifier : ASTLValue
    {
        public String ID { get; set; }
        public Descriptor SelfDescriptor { get; set; }
        public string ActualModifier { get; set; }

        public ASTIdentifier(LexLocation loc, String id, string actualModifier = null)
        {
            Location = loc;
            ID = id;
            ActualModifier = actualModifier;
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
