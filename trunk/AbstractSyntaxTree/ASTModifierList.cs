using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTModifierList : ASTNode
    {
        public bool IsEmpty { get; set; }
        public string Modifier { get; set; }
        public ASTModifierList Tail { get; set; }

        public ASTModifierList ()
        {
            IsEmpty = true;
        }

        public ASTModifierList (string mod, ASTModifierList tail)
        {
            Modifier = mod;
            Tail = tail;
            IsEmpty = false;
        }

        public override string Print (int depth)
        {
            if (IsEmpty)
                return string.Empty;

            return Modifier + " " + Tail.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitModifierList(this);
        }
    }
}
