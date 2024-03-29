﻿using System;
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
        public override bool IsType { get { return true; } }
        public List<string> Modifiers { get; private set; }
        public string Name { get; private set; }
        public ClassDescriptor ContainingClass { get; set; }

        public MemberDescriptor(CFlatType type, string name, ClassDescriptor cls)
            : base(type)
        {
            Modifiers = new List<string>();
            Name = name;
            ContainingClass = cls;
        }
    }
}
