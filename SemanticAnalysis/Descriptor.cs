using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public abstract class Descriptor
    {
        public CFlatType Type { get; private set; }
        public virtual bool IsType { get { return false; } }
        public virtual bool IsObject { get { return false; } }
        public virtual bool IsField { get { return false; } }
        public virtual bool IsMethod { get { return false; } }

        public Descriptor(CFlatType t) { this.Type = t; }
    }
}
