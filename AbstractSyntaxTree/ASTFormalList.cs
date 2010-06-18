using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTFormalList : ASTNode
    {
        public bool IsEmpty { get; set; }
        public ASTFormal Formal { get; set; }
        public ASTFormalList Tail { get; set; }

        public ASTFormalList ()
        {
            IsEmpty = true;
        }
        public ASTFormalList (ASTFormal formal, ASTFormalList tail)
        {
            IsEmpty = false;
            Formal = formal;
            Tail = tail;
        }

        public override string Print (int depth)
        {
            if (IsEmpty)
                return "";

            if (Tail.IsEmpty)
                return Formal.Print(depth).ToString();
            else
                return Formal.Print(depth) + "," + Tail.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitFormalList(this);
        }
    }
}
