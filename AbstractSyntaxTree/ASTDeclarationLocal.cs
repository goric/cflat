using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTDeclarationLocal : ASTStatement
    {
        public ASTType Type { get; set; }
        public String ID { get; set; }
        public bool HasValue { get; set; }
        public ASTExpression InitialValue { get; set; }

        public ASTDeclarationLocal(SourceLocation location, ASTType type, String id)
        {
            HasValue = false;
            Type = type; 
            ID = id;
            Location = location;
        }
        public ASTDeclarationLocal (SourceLocation location, ASTType type, String id, ASTExpression value)
        {
            HasValue = true;
            Type = type;
            ID = id;
            Location = location;
            InitialValue = value;
        }

        public override String Print(int depth)
        {
            if(!HasValue)
                return Type.Print(depth) + " " + ID;

            return Type.Print(depth) + " " + ID + " = " + InitialValue.Print(depth);
        }

        public override void Visit (Visitor v)
        {
            v.VisitDeclLocal(this);
        }
    }
}
