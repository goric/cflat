using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTDeclarationField : ASTDeclaration
    {
        public ASTType FieldType { get; set; }
        public String Name { get; set; }
        public Type Type { get; set; }
        public Descriptor Descriptor { get; set; }

        public ASTDeclarationField (ASTType type, String name)
        {
            FieldType = type;
            Name = name;
        }

        public override String Print (int depth)
        {
            return FieldType.Print(depth) + " " + Name;
        }

        public override void Visit (Visitor v)
        {
            v.VisitDeclField(this);
        }
    }
}
