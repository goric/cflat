using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeFunction : CFlatType
    {
        public override bool IsFunction { get { return true; } }
        public bool IsConstructor { get; set; }
        public Scope Scope { get; set; }
        public CFlatType ReturnType { get; set; }

        public Dictionary<string, CFlatType> Formals;
        public Dictionary<string, CFlatType> Locals;

        public TypeFunction()
            : this(false)
        {

        }

        public TypeFunction(bool isCtor)
        {
            IsConstructor = isCtor;
            Formals = new Dictionary<string, CFlatType>();
            Locals = new Dictionary<string, CFlatType>();
        }

        public void AddFormal(string name, CFlatType type)
        {
            Formals.Add(name, type);
        }

        public void AddLocal(string name, CFlatType type)
        {
            Locals.Add(name, type);
        }

        public override bool IsSupertype(TypeFunction checkType)
        {
            throw new NotImplementedException();
        }

        public override string ToString() { return ""; }
    }
}
