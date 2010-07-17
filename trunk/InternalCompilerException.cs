using System;

namespace CFlat
{
    public class InternalCompilerException : Exception
    {
        public InternalCompilerException() : base() { }
        public InternalCompilerException(string msg) : base(msg) { }
    }
}
