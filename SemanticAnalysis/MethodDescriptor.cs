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
        public bool IsCFlatMethod { get; set; }
        public string Name { get; set; }
        public List<String> Modifiers { get; private set; }
        public List<FormalDescriptor> Formals { get; private set; }
        public ClassDescriptor ContainingClass { get; private set; }

        public MethodDescriptor(CFlatType t, string name, ClassDescriptor cls) : base(t)
        {
            IsCompleted = false;
            Modifiers = new List<string>();
            Formals = new List<FormalDescriptor>();
            Name = name;
            ContainingClass = cls;
        }

        public override string ToString()
        {
            return "";
        }
    }
}
