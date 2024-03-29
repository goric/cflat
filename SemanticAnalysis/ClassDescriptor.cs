﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class ClassDescriptor : Descriptor
    {
        public override bool IsType { get { return true; } }
        public ClassDescriptor ParentClass { get; private set; }
        public List<MethodDescriptor> Methods { get; private set; }
        public List<MemberDescriptor> Fields { get; private set; }
        public string Name { get; private set; }
        public string InstanceName { get; set; }
        public Scope Scope { get; set; }

        public ClassDescriptor(CFlatType t, ClassDescriptor parentClass, string name, Scope s) 
            : base(t)
        {
            ParentClass = parentClass;
            Methods = new List<MethodDescriptor>();
            Fields = new List<MemberDescriptor>();
            Name = name;
            Scope = s;
        }

        public override string ToString()
        {
            return "CFlatType_Class<" + this.Type.ToString() + ">";
        }
    }
}
