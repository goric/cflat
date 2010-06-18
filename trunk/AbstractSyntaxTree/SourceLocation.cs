using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public class SourceLocation
    {
        public int Line { get; set; }
        public int Col { get; set; }
        public String File { get; set; }

        public SourceLocation (int line, int column, String file)
        {
            Line = line;
            Col = column;
            File = file;
        }

        public override String ToString ()
        {
            return File + ":" + Line + ":" + Col;
        }
    }
}
