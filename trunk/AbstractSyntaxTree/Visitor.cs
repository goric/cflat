using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AbstractSyntaxTree
{
    public abstract class Visitor
    {
        public bool DebugMode { get; set; }

        public virtual void VisitNode(ASTNode n) { }

        #region Types
        /* Variables */
        public virtual void VisitType (ASTType n) { }
        public virtual void VisitTypeArray (ASTTypeArray n) { }
        public virtual void VisitTypeBool (ASTTypeBool n) { }
        public virtual void VisitTypeString (ASTTypeString n) { }
        public virtual void VisitTypeInt (ASTTypeInt n) { }
        public virtual void VisitTypeReal (ASTTypeReal n) { }
        public virtual void VisitTypeName (ASTTypeName n) { }
        public virtual void VisitTypeVoid (ASTTypeVoid n) { }
        /* Literals */
        public virtual void VisitIdentifier (ASTIdentifier n) { }
        public virtual void VisitInteger (ASTInteger n) { }
        public virtual void VisitReal (ASTReal n) { }
        public virtual void VisitString (ASTString n) { }
        public virtual void VisitBoolean (ASTBoolean n) { }

        #endregion

        #region Expressions

        public virtual void VisitExpr (ASTExpression n) { }
        public virtual void VisitExprList (ASTExpressionList n)
        {
            if (!n.IsEmpty)
            {
                n.Expr.Visit(this);
                n.Tail.Visit(this);
            }
        }

        #region Binary Operations

        public virtual void VisitBinary (ASTBinary n)
        {
            n.Left.Visit(this);
            n.Right.Visit(this);
        }

        public virtual void VisitGreater (ASTGreater n) { }
        public virtual void VisitGreaterEqual (ASTGreaterEqual n) { }
        public virtual void VisitSmaller (ASTSmaller n) { }
        public virtual void VisitSmallerEqual (ASTSmallerEqual n) { }
        public virtual void VisitEqual (ASTEqual n) { }
        public virtual void VisitNotEqual (ASTNotEqual n) { }

        public virtual void VisitMul (ASTMultiply n) { }
        public virtual void VisitDiv (ASTDivide n) { }
        public virtual void VisitMod (ASTModulo n) { }
        public virtual void VisitAdd (ASTAdd n) { }
        public virtual void VisitSub (ASTSubtract n) { }
        public virtual void VisitAnd (ASTAnd n) { }
        public virtual void VisitOr (ASTOr n) { }
        public virtual void VisitExponent (ASTExponent n) { }

        #endregion

        #region Unary Operations

        public virtual void VisitIncrement (ASTIncrement n) { }
        public virtual void VisitDecrement (ASTDecrement n) { }
        public virtual void VisitNeg (ASTNegative n) { }
        public virtual void VisitNot (ASTNot n) { }

        #endregion

        #endregion

        #region Class/Method Operations

        public virtual void VisitClassDefinition (ASTClassDefinition n) { }
        public virtual void VisitSubClassDefinition (ASTSubClassDefinition n) { }
        public virtual void VisitInstantiateClass (ASTInstantiateClass n) { }

        public virtual void VisitInvoke (ASTInvoke n) { }
        public virtual void VisitSelf (ASTSelf n) { }
        public virtual void VisitBase (ASTBase n) { }
        public virtual void VisitDerefField (ASTDereferenceField n) { }

        public virtual void VisitDerefArray (ASTDereferenceArray n) { }
        public virtual void VisitInstantiateArray (ASTInstantiateArray n) { }

        public virtual void VisitFormal (ASTFormal n) { }
        public virtual void VisitFormalList (ASTFormalList n)
        {
            if (!n.IsEmpty)
            {
                n.Formal.Visit(this);
                n.Tail.Visit(this);
            }
        }

        public virtual void VisitModifierList (ASTModifierList n) { }

        #endregion

        #region Declarations

        public virtual void VisitDecl(ASTDeclaration n) { }
        public virtual void VisitDeclLocal (ASTDeclarationLocal n) { }
        public virtual void VisitDeclField (ASTDeclarationField n) { }
        public virtual void VisitDeclMethod (ASTDeclarationMethod n) { }
        public virtual void VisitDeclConstructor (ASTDeclarationCtor n) { }

        public virtual void VisitDeclList (ASTDeclarationList n)
        {
            if (!n.IsEmpty)
            {
                n.Declaration.Visit(this);
                n.Tail.Visit(this);
            }
        }

        #endregion

        #region Statements

        public virtual void VisitStatement (ASTStatement n) { }
        public virtual void VisitAssign (ASTAssign n) { }
        public virtual void VisitWhile (ASTWhile n) { }
        public virtual void VisitFor (ASTFor n) { }
        public virtual void VisitForIn (ASTForIn n) { }
        public virtual void VisitReturn (ASTReturn n) { }
        public virtual void VisitNoop (ASTNoop n) { }
        public virtual void VisitBlock (ASTBlock n) { }
        public virtual void VisitIfThen (ASTIfThen n) { }
        public virtual void VisitIfThenElse (ASTIfThenElse n) { }
        public virtual void VisitStatementExpr (ASTStatementExpr n) { }

        public virtual void VisitStatementList (ASTStatementList n)
        {
            if (!n.IsEmpty)
            {
                n.Statement.Visit(this);
                n.Tail.Visit(this);
            }
        }

        #endregion
    }
}
