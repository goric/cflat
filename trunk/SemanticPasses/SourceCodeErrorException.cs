using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.SemanticPasses
{
    public class SourceCodeErrorException : Exception
    {
        public SourceCodeErrorException() : base() { }
        public SourceCodeErrorException(string msg) : base(msg) { }
    }
}
