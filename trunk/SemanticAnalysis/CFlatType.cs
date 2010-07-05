using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public abstract class CFlatType
    {
        public virtual bool IsSupertype(CFlatType checkType) { return false; }
        public virtual bool IsSupertype(TypeInt checkType) { return false; }
        public virtual bool IsSupertype(TypeBool checkType) { return false; }
        public virtual bool IsSupertype(TypeReal checkType) { return false; }
        public virtual bool IsSupertype(TypeString checkType) { return false; }
        public virtual bool IsSupertype(TypeVoid checkType) { return false; }
        
        public virtual bool IsArray { get{ return false; } }
        public virtual bool IsInstance { get { return false; } }
        public virtual bool IsFunction { get { return false; } }
        public virtual bool IsClass { get { return false; } }

        public virtual int Size { get { return 0; } }

        public virtual CFlatType BaseType { get { return this; } set { /* do nothing */} }
    }
}
