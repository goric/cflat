using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeFunction : CFlatType
    {
        public override bool IsFunction { get{ return true; } }
        public Scope Scope { get; set; }
        public CFlatType ReturnType { get; set; }

        public Dictionary<string, CFlatType> _formals;
        public TypeFunction ()
        {
            _formals = new Dictionary<string, CFlatType>();
        }

        public void AddFormal (string name, CFlatType type)
        {
            _formals.Add(name, type);
        }
        public override string ToString() { return ""; }
    }
}
