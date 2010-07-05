using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTSubClassDefinition : ASTStatement
    {
        public String Name { get; set; }
        public String Parent { get; set; }
        public ASTDeclarationList Declarations { get; set; }
        public TypeClass Type { get; set; }
        public ClassDescriptor Descriptor { get; set; }

        public ASTSubClassDefinition (String name, String parent, ASTDeclarationList decls)
        {
            Name = name;
            Parent = parent;
            Declarations = decls;
        }
        
        public override String Print(int depth)
        {
            return "class " + Name + " is " + Parent + "{"
                            + NewLine(depth + 1) + Declarations.Print(depth + 1) + NewLine(depth) + "}";
        }

        public override void Visit (Visitor v)
        {
            v.VisitSubClassDefinition(this);
        }
    }
}
