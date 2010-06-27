using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTStatementList : ASTStatement
    {
        public bool IsEmpty { get; set; }
        public ASTStatement Statement { get; set; }
        public ASTStatementList Tail { get; set; }

        public ASTStatementList ()
        {
            IsEmpty = true;
        }
        public ASTStatementList (ASTStatement statement, ASTStatementList tail)
        {
            IsEmpty = false;
            Statement = statement;
            Tail = tail;
        }

        public override String Print (int depth)
        {
            if (IsEmpty)
                return "";

            if (Tail.IsEmpty)
                return Statement.Print(depth) + ";";
            else
                return Statement.Print(depth) + ";" + NewLine(depth) + Tail.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitStatementList(this);
        }
    }
}
