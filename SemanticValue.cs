using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AbstractSyntaxTree;

namespace CFlat
{
    /// <summary>
    /// Struct used in the Parser generator as the TValue input for the ShiftReduceParser base class.
    /// This allows to strongly type both the terminals and the non-terminals in the grammar input file
    /// so each semantic action defined will produce an ASTNode of the proper type.
    /// </summary>
    internal struct SemanticValue
    {
        public Token Token { get; set; }

        public ASTStatementList StatementList { get; set; }
        public ASTStatement Statement { get; set; }
        public ASTDeclarationList DeclarationList { get; set; }
        public ASTDeclaration Declaration { get; set; }
        public ASTFormalList FormalList { get; set; }
        public ASTFormal Formal { get; set; }
        public ASTExpressionList ExpressionList { get; set; }
        public ASTExpression Expression { get; set; }
        public ASTLValue LValue { get; set; }
        public ASTType Type { get; set; }
        public ASTModifierList ModifierList { get; set; }
    }
}
