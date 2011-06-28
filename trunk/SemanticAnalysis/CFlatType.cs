using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public abstract class CFlatType
    {
        public virtual bool IsSupertype(CFlatType checkType)
        {
            /* This method should never return a value itself, but rather call the appropriate overload for the
             *  concrete type of checkType */
            dynamic realType = checkType;
            return this.IsSupertype(realType);
        }

        public virtual bool IsSupertype(TypeArray checkType) { return false; }
        public virtual bool IsSupertype(TypeBool checkType) { return false; }
        public virtual bool IsSupertype(TypeClass checkType) { return false; }
        public virtual bool IsSupertype(TypeFunction checkType) { return false; }
        public virtual bool IsSupertype(TypeInt checkType) { return false; }
        public virtual bool IsSupertype(TypeReal checkType) { return false; }
        public virtual bool IsSupertype(TypeString checkType) { return false; }
        public virtual bool IsSupertype(TypeVoid checkType) { return false; }
        public virtual bool IsSupertype(TypeChar checkType) { return false; }
        
        public virtual bool IsArray { get{ return false; } }
        public virtual bool IsInstance { get { return false; } }
        public virtual bool IsFunction { get { return false; } }
        public virtual bool IsClass { get { return false; } }
        public virtual bool IsNumeric { get { return false; } }
        public virtual bool IsString { get { return false; } }
        public virtual bool IsChar { get { return false; } }
        public virtual bool IsConcatenatable { get { return false; } }
        public virtual bool NeedsBoxing { get { return false; } }

        public virtual int Size { get { return 0; } }

        public abstract Type CilType { get; }

        public virtual CFlatType BaseType { get { return this; } set { /* do nothing */} }

        public bool IsSubtypeOf(CFlatType t)
        {
            return t.IsSupertype(this);
        }
    }
}
