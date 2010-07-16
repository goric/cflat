using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    /// <summary>
    /// Holds information for member variables
    /// </summary>
    public class MemberDescriptor : Descriptor
    {
        public MemberDescriptor(CFlatType type)
            : base(type)
        {

        }

        public override bool IsType { get { return true; } }
    }
}
