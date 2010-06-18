using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AbstractSyntaxTree;
using SyntaxAnalysis;

namespace CFlat
{
    internal class Token
    {
        public string Value { get; set; }
        public SourceLocation Source { get; set; }
        public Tokens TokenType { get; set; }

        public Token (Tokens type)
        {
            this.TokenType = type;
        }
        public Token (Tokens type, string value)
        {
            this.TokenType = type;
            Value = value;
        }
        public Token (Tokens type, string value, int line, int column, string file = "")
        {
            Value = value;
            Source = new SourceLocation(line, column, file);
        }
    }
}
