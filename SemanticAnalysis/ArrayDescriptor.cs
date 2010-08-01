using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class ArrayDescriptor : Descriptor
    {
        public CFlatType Type { get; set; }
        public CFlatType BaseType { get; set; }
        public String Name { get; private set; }
        public int LowerBound { get; private set; }
        public int UpperBound { get; private set; }

        public ArrayDescriptor (CFlatType type, CFlatType baseType, string name, int lower, int upper)
            : base(type)
        {
            Type = type;
            BaseType = baseType;
            Name = name;
            LowerBound = lower;
            UpperBound = upper;
        }
    }
}
