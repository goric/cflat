using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    /// <summary>
    /// A class that represents an array of any type. This will allow methods to accept an array of any
    /// type as an argument and pass the type checking in the third pass.
    /// </summary>
    public class TypeAnyArray : CFlatType
    {
        public override bool IsArray
        {
            get
            {
                return true;
            }
        }

        public override bool IsSupertype(TypeArray checkType)
        {
            return true;
        }

        public override Type CilType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
