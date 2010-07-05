using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTClassDefinition : ASTStatement
    {
        public String Name { get; set; }
        public ASTDeclarationList Declarations { get; set; }
        public TypeClass Type { get; set; }
        public ClassDescriptor Descriptor { get; set; }

        public ASTClassDefinition (String name, ASTDeclarationList decls)
        {
            Name = name;
            Declarations = decls;
        }
        
        public override String Print(int depth)
        {
            return "class " + Name + "{" + NewLine(depth + 1) + Declarations.Print(depth + 1) + NewLine(depth) + "}";
        }

        public override void Visit (Visitor v)
        {
            v.VisitClassDefinition(this);
        }
    }
}
