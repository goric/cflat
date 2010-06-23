using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class ClassDescriptor : Descriptor
    {
        public override bool IsType
        {
            get
            {
                return true;
            }
        }

        
        public ClassDescriptor(CFlatType t, ClassDescriptor parentClass) : base(t) { ParentClass = parentClass; }

        public ClassDescriptor ParentClass { get; private set; }

        public override string ToString()
        {
            return "CFlatType_Class<" + this.Type.ToString() + ">";
        }

        
    }
}
