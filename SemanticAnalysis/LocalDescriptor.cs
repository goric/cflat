using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    /// <summary>
    /// Holds information for local variables
    /// </summary>
    public class LocalDescriptor : Descriptor
    {
        public string Name { get; set; }

        public override bool IsType { get { return true; } }

        public LocalDescriptor(CFlatType type, string name)
            : base(type)
        {
            Name = name;
        }
    }
}
