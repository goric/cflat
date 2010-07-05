using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class MethodDescriptor : Descriptor
    {
        public override bool IsMethod { get { return true; } }
        public bool IsCompleted { get; private set; }

        public MethodDescriptor(CFlatType t) : base(t)
        {
            IsCompleted = false;
        }

        public override string ToString()
        {
            return "";
        }
    }
}
