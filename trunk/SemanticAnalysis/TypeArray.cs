using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeArray : CFlatType
    {
        public override bool IsArray
        {
            get
            {
                return true;
            }
        }
        public override int Size
        {
            get
            {
                return _baseType.Size;
            }
        }

        private CFlatType _baseType;
        public TypeArray (CFlatType baseType)
        {
            _baseType = baseType;
        }

        public virtual CFlatType BaseType { get { return _baseType; } set { /* do nothing */} }
    }
}
