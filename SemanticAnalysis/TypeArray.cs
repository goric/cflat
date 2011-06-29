using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeArray : TypeAnyArray
    {
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

        /// <summary>
        /// Returns true when the type of the elements in the arrays are the same.
        /// </summary>
        /// <param name="checkType"></param>
        /// <returns></returns>
        public override bool IsSupertype(TypeArray checkType)
        {
            return this.BaseType.IsSupertype(checkType.BaseType) && checkType.BaseType.IsSupertype(this.BaseType);
        }

        public override CFlatType BaseType { get { return _baseType; } set { /* do nothing */} }

        public override Type CilType
        {
            get { return BaseType.CilType.MakeArrayType(); }
        }
    }
}
