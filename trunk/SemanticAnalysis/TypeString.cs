using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeString : CFlatType
    {
        /// <summary>
        /// Max string length - 65k?
        /// </summary>
        public override int Size
        {
            get
            {
                return Int32.MaxValue;
            }
        }

        public override string ToString() { return "string"; }
    }
}
