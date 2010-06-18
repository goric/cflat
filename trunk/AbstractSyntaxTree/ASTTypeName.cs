using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class ASTTypeName : ASTType
    {
        public String Name { get; set; }

        public ASTTypeName (String name)
        {
            Name = name;
        }

        public override String Print (int depth)
        {
            return Name;
        }

        public override void Visit (Visitor v)
        {
            v.VisitTypeName(this);
        }
    }
}
