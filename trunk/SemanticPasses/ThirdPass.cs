using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    public class ThirdPass : SecondPass
    {
        private TypeFunction _currentMethod;

        public ThirdPass(ASTNode treeNode, ScopeManager mgr)
            : base(treeNode, mgr)
        {

        }

        #region Types

        public override void VisitIdentifier(ASTIdentifier n)
        {

        }

        #endregion

        #region Expressions

        public override void VisitExpr(ASTExpression n)
        {

        }

        #endregion

        #region Binary Operators

        public override void VisitGreater(ASTGreater n)
        { 

        }

        public override void VisitGreaterEqual(ASTGreaterEqual n)
        { 

        }

        public override void VisitSmaller(ASTSmaller n)
        { 

        }

        public override void VisitSmallerEqual(ASTSmallerEqual n)
        { 

        }

        public override void VisitEqual(ASTEqual n)
        { 

        }

        public override void VisitNotEqual(ASTNotEqual n)
        { 

        }

        public override void VisitMul(ASTMultiply n)
        { 

        }

        public override void VisitDiv(ASTDivide n)
        { 

        }

        public override void VisitMod(ASTModulo n)
        {

        }

        public override void VisitAdd(ASTAdd n)
        { 

        }

        public override void VisitSub(ASTSubtract n)
        {

        }

        public override void VisitAnd(ASTAnd n)
        { 

        }

        public override void VisitOr(ASTOr n)
        {

        }

        public override void VisitExponent(ASTExponent n)
        {

        }

        #endregion

        #region Unary Operations

        public override void VisitIncrement(ASTIncrement n) 
        { 
            
        }

        public override void VisitDecrement(ASTDecrement n)
        { 

        }

        public override void VisitNeg(ASTNegative n)
        {

        }

        public override void VisitNot(ASTNot n)
        { 

        }

        #endregion

        #region Statements

        public override void VisitStatement(ASTStatement n) 
        { 
            
        }

        public override void VisitAssign(ASTAssign n) 
        {

        }

        public override void VisitWhile(ASTWhile n)
        {

        }

        public override void VisitFor(ASTFor n) 
        {

        }

        public override void VisitForIn(ASTForIn n)
        {

        }

        public override void VisitReturn(ASTReturn n) 
        {

        }

        public override void VisitNoop(ASTNoop n) 
        {

        }

        public override void VisitBlock(ASTBlock n) 
        { 

        }

        public override void VisitIfThen(ASTIfThen n)
        { 

        }

        public override void VisitIfThenElse(ASTIfThenElse n) 
        { 

        }

        public override void VisitStatementExpr(ASTStatementExpr n) 
        { 

        }

        #endregion

        #region Class/Methods

        /// <summary>
        /// Restore the scope of the class we're visiting, and then visit all of the child nodes.
        /// Pop the class scope when we're done.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            VisitClassBody(n.Type as TypeClass, n.Declarations);
        }

        /// <summary>
        /// Same as visiting a regular Class definition.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            VisitClassBody(n.Type as TypeClass, n.Declarations);
        }

        public override void VisitInstantiateClass(ASTInstantiateClass n)
        {
            ClassDescriptor desc = _scopeMgr.GetType(n.ClassName) as ClassDescriptor;
            if (desc != null)
            {
                TypeClass currentClass = (TypeClass)desc.Type;
                //Check if the class we're working with has a constructor of the same name
                TypeFunction ctor = currentClass.Methods[n.ClassName] as TypeFunction;
                if (ctor != null)
                {
                    CheckSubTree(n.Actuals);
                    //check the method signature of the constructor to make sure the correct arguments are passed in
                    ActualBuilder builder = new ActualBuilder();
                    n.Actuals.Visit(builder);

                    if (ctor.AcceptCall(builder.Actuals))
                    {
                        //hooray, the code is valid (I think)
                        MethodDescriptor ctorDescriptor = (MethodDescriptor)currentClass.Scope.Descriptors[n.ClassName];
                        _lastSeenType = currentClass;
                        n.ClassDescriptor = desc;
                        n.Descriptor = ctorDescriptor;
                    }
                    else
                        ReportError(null, "Invalid parameters for constructor '{0}'", n.ClassName);
                }
                else
                    ReportError(null, "No constructor found for class '{0}'.", n.ClassName);
            }
            else
                ReportError(null, "The name '{0}' is not a class.", n.ClassName);
        }

        /// <summary>
        /// Checks to make sure that both the upper and lower bounds are integers
        /// </summary>
        /// <param name="n"></param>
        public override void VisitInstantiateArray(ASTInstantiateArray n)
        {
            if ((CheckSubTree(n.Upper) is TypeInt) && (CheckSubTree(n.Lower) is TypeInt))
            {
                CFlatType elementType = CheckSubTree(n.Type);

                TypeArray arrType = new TypeArray(elementType);
                _lastSeenType = arrType;
                n.CFlatType = arrType;
            }
            else
                ReportError(null, "Bounds of an array must be integers.");
        }

        #endregion

        #region Declarations

        /// <summary>
        /// Intentionally left blank. Declarations are processed during the second pass, 
        /// so this function will do nothing to avoid adding declarations twice to the underlying dictionary.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclField(ASTDeclarationField n)
        {
            //nothing to do here, already done in SecondPass
        }

        /// <summary>
        /// Restores the method's scope, and adds a new scope for local variables.
        /// Walks the body of the function and pops the two scopes.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclMethod(ASTDeclarationMethod n)
        {
            VisitMethodBody(string.Format("body {0}", n.Name), n.Body, n.Type as TypeFunction);
        }

        /// <summary>
        /// Pretty much the same thing as visiting a regular method.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclConstructor(ASTDeclarationCtor n)
        {
            VisitMethodBody(string.Format("ctor body {0}", n.Name), n.Body, n.Type as TypeFunction);
        }

        /// <summary>
        /// Checks if the variable already exists. This does not allow for shadowing of variables, maybe as an
        /// enhancement, cause it wouldn't be that hard.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclLocal(ASTDeclarationLocal n)
        {
            if (!_scopeMgr.HasSymbol(n.ID))
            {
                CFlatType t = CheckSubTree(n.Type);
                _scopeMgr.AddLocal(n.ID, t, _currentMethod);
            }
            else
                ReportError(n.Location, "The identifier '{0}' already exists", n.ID);
        }

        #endregion

        #region Private helper methods

        /// <summary>
        /// Restore the scope of the class we're visiting, and then visit all of the child nodes.
        /// Pop the class scope when we're done.
        /// 
        /// Both VisitClassDefinition and VisitSubClassDefinition do the same thing, so moved the code out into 1 method.
        /// </summary>
        private void VisitClassBody(TypeClass currentClass, ASTDeclarationList declarations)
        {
            _scopeMgr.RestoreScope(currentClass.Scope);

            CheckSubTree(declarations);

            _scopeMgr.PopScope();
        }

        /// <summary>
        /// Restores the method's scope, and adds a new scope for local variables.
        /// Walks the body of the function and pops the two scopes.
        /// 
        /// Both VisitDeclMethod and VisitDeclConstructor do the same thing, so moved the code out into 1 method.
        /// </summary>
        /// <param name="scopeName"></param>
        /// <param name="body"></param>
        /// <param name="f"></param>
        private void VisitMethodBody(string scopeName, ASTStatementList body, TypeFunction f)
        {
            _currentMethod = f;
            //restore the scope of the formals
            _scopeMgr.RestoreScope(_currentMethod.Scope);
            //add a new scope for any locals
            Scope localScope = _scopeMgr.PushScope(scopeName);

            CheckSubTree(body);

            //pop the body scope and the formal scope
            _scopeMgr.PopScope();
            _scopeMgr.PopScope();
        }

        private bool CheckMethodSignature(TypeFunction fn, List<CFlatType> actuals)
        {
            List<CFlatType> formals = fn.Formals.Values.OfType<CFlatType>().ToList<CFlatType>();
            if (formals.Count != actuals.Count)
                return false;
            else
            {
                for (int i = 0; i < formals.Count; i++)
                {
                   //need to check if the formal is a supertype of the actual
                }

                return true;
            }
        }

        #endregion
    }
}
