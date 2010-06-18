using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTDeclarationList : ASTNode
    {
        public bool IsEmpty { get; set; }
        public ASTDeclaration Declaration { get; set; }
        public ASTDeclarationList Tail { get; set; }

        public ASTDeclarationList ()
        {
            IsEmpty = true;
        }
        public ASTDeclarationList (ASTDeclaration decl, ASTDeclarationList tail)
        {
            IsEmpty = false;
            Declaration = decl;
            Tail = tail;
        }

        public override String Print (int depth)
        {
            if (IsEmpty)
                return "";

            if (Tail.IsEmpty)
                return Declaration.Print(depth);
            else
                return Declaration.Print(depth) + ";" + NewLine(depth) + Tail.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitDeclList(this);
        }	
    }
}
