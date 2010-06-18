using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SemanticAnalysis;

namespace AbstractSyntaxTree
{
    public class ASTDeclarationCtor : ASTDeclaration
    {
        public String Name { get; set; }
        public ASTFormalList Formals { get; set; }
        public ASTStatementList Body { get; set; }
        public Type Type { get; set; }
        public MethodDescriptor Descriptor { get; set; }

        public ASTDeclarationCtor(String name, ASTFormalList formals, ASTStatementList body)
        {
            Name = name;
            Formals = formals;
            Body = body;
        }

        public override String Print(int depth)
        {
            return Name + "(" + Formals.Print(depth) + ")"
                //	+ " : " + _tty.toString() 
                    + "{" + NewLine(depth + 1) + Body.Print(depth + 1) + NewLine(depth) + "}";
        }

        public override void Visit (Visitor v)
        {
            v.VisitDeclConstructor(this);
        }
    }
}
