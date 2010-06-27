using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTFormal : ASTNode
    {
        public string Modifier { get; set; }
        public ASTType Type { get; set; }
        public String Name { get; set; }

        public ASTFormal (ASTType type, String name)
        {
            Type = type;
            Name = name;
        }
        public ASTFormal (string modifier, ASTType type, String name)
        {
            Modifier = modifier;
            Type = type;
            Name = name;
        }
        
        public override String Print(int depth)
        {
            return Modifier + " " + Type.Print(depth) + " " + Name;
        }

        public override void Visit (Visitor v)
        {
            v.VisitFormal(this);
        }
    }
}
