using System;

namespace CFlat.SemanticPasses
{
    public class SourceCodeErrorException : Exception
    {
        public SourceCodeErrorException() : base() { }
        public SourceCodeErrorException(string msg) : base(msg) { }
    }
}
