using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    public class ThirdPass : SecondPass, ICompilerPass
    {
        private TypeFunction _currentMethod;

        public ThirdPass(ASTNode treeNode, ScopeManager mgr)
            : base(treeNode, mgr)
        {

        }

        new public string PassName()
        {
            return "Third Pass";
        }

        #region Types

        /// <summary>
        /// Makes sure the identifier has been declared and that is is actually a type of some kind (i.e. local, member, not a method).
        /// Sets the AST node's type and descriptor if it is declared.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitIdentifier(ASTIdentifier n)
        {
            if (_scopeMgr.HasSymbol(n.ID))
            {
                Descriptor d = _scopeMgr.GetType(n.ID);
                if (d != null)
                {
                    n.Descriptor = d;
                    n.CFlatType = d.Type;

                    _lastSeenType = d.Type;
                }
                else
                {
                    ReportError(n.Location, "Identifier '{0}' is not a type (it's probably a method or something).", n.ID);
                }
            }
            else
            {
                ReportError(n.Location, "Identifier '{0}' has not been declared.", n.ID);
            }
        }

        /// <summary>
        /// Integer literal
        /// </summary>
        /// <param name="n"></param>
        public override void VisitInteger(ASTInteger n)
        {
            n.CFlatType = _lastSeenType = new TypeInt();
        }

        /// <summary>
        /// Real literal
        /// </summary>
        /// <param name="n"></param>
        public override void VisitReal(ASTReal n)
        {
            n.CFlatType = _lastSeenType = new TypeReal();
        }

        /// <summary>
        /// String literal
        /// </summary>
        /// <param name="n"></param>
        public override void VisitString(ASTString n)
        {
            n.CFlatType = _lastSeenType = new TypeString();
        }

        /// <summary>
        /// Boolean literal
        /// </summary>
        /// <param name="n"></param>
        public override void VisitBoolean(ASTBoolean n)
        {
            n.CFlatType = _lastSeenType = new TypeBool();
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

        /// <summary>
        /// Makes sure the left hand side of the assignment is a supertype of the right hand side.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitAssign(ASTAssign n) 
        {
            CFlatType lhs = CheckSubTree(n.LValue);
            CFlatType rhs = CheckSubTree(n.Expr);

            if (rhs.IsSupertype(lhs))
            {
                //I believe we don't really do anything when the source code is correct in this case.
                n.CFlatType = new TypeVoid();
                _lastSeenType = n.CFlatType;
            }
            else
            {
                ReportError(n.Location, "Invalid assignment, type mismatch. Expected: {0} Got: {1}", TypeToFriendlyName(lhs), TypeToFriendlyName(rhs));
            }
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
                        //hooray, the code is valid
                        MethodDescriptor ctorDescriptor = (MethodDescriptor)currentClass.Scope.Descriptors[n.ClassName];
                        _lastSeenType = currentClass;
                        n.ClassDescriptor = desc;
                        n.Descriptor = ctorDescriptor;
                    }
                    else
                        ReportError(n.Location, "Invalid parameters for constructor '{0}'", n.ClassName);
                }
                else
                    ReportError(n.Location, "No constructor found for class '{0}'.", n.ClassName);
            }
            else
                ReportError(n.Location, "The name '{0}' is not a class.", n.ClassName);
        }

        /// <summary>
        /// VisitInvoke and VisitDereferenceField are very similiar, so this should probably be refactored out
        /// into a common method.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitInvoke(ASTInvoke n)
        {
            //Make sure the lvalue is a type and the method name exists
            CFlatType lhs = CheckSubTree(n.Object);
            if (lhs.IsClass)
            {
                TypeClass lvalue = (TypeClass)lhs;
                //check if a method with the given name exists in the scope.
                //This needs to check not only the class's shallow scope, but all the parents as well.
                if (lvalue.Scope.HasSymbol(n.Method))
                {
                    MethodDescriptor methodDesc = lvalue.Scope.Descriptors[n.Method] as MethodDescriptor;
                    if (methodDesc != null)
                    {
                        //check if the arguments match
                        TypeFunction method = (TypeFunction)methodDesc.Type;
                        //visit any actuals that need processing
                        CheckSubTree(n.Actuals);
                        //collect the actuals
                        ActualBuilder builder = new ActualBuilder();
                        n.Actuals.Visit(builder);

                        if (method.AcceptCall(builder.Actuals))
                        {
                            n.Descriptor = methodDesc;
                            n.CFlatType = methodDesc.Type;

                            _lastSeenType = methodDesc.Type;
                        }
                        else
                        {
                            ReportError(n.Location, "Invalid parameters for method  '{0}.{1}'", TypeToFriendlyName(lvalue), n.Method);
                        }
                    }
                    else
                    {
                        ReportError(n.Location, "'{0}' is not a method for type '{1}'", n.Method, TypeToFriendlyName(lvalue));
                    }
                }
                else
                {
                    ReportError(n.Location, "Method '{0}' does not exist for type '{1}'", n.Method, TypeToFriendlyName(lvalue));
                }
            }
            else
            {
                ReportError(n.Location, "Type '{0}' does not support methods.", TypeToFriendlyName(lhs));
            }
        }

        public override void VisitSelf(ASTSelf n)
        {
            n.CFlatType = _currentClass;
            //set n.Descriptor if it hasn't already been set?

            _lastSeenType = _currentClass;
        }

        public override void VisitBase(ASTBase n) 
        {

        }

        /// <summary>
        /// For now, this will only allow Classes to have fields and methods.
        /// So primitives like int, real, bool won't have any.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDerefField(ASTDereferenceField n)
        {
            //we're processing something of the form: lvalue.identifier

            //Make sure the lvalue is a type and the identifier exists
            CFlatType lhs = CheckSubTree(n.Object);
            if (lhs.IsClass)
            {
                TypeClass lvalue = (TypeClass)lhs;
                //check if a field exists.
                if (lvalue.Scope.HasSymbol(n.Field))
                {
                    MemberDescriptor fieldDesc = lvalue.Scope.Descriptors[n.Field] as MemberDescriptor;
                    if (fieldDesc != null)
                    {
                        //hooray, code is valid
                        n.Descriptor = fieldDesc;
                        n.CFlatType = fieldDesc.Type;

                        _lastSeenType = fieldDesc.Type;
                    }
                    else
                    {
                        ReportError(n.Location, "'{0}' is not a field for type '{1}'", n.Field, TypeToFriendlyName(lvalue));
                    }
                }
                else
                {
                    ReportError(n.Location, "Field '{0}' does not exist for type '{1}'", n.Field, TypeToFriendlyName(lvalue)); 
                }
            }
            else
            {
                ReportError(n.Location, "Type '{0}' does not support fields.", TypeToFriendlyName(lhs));
            }
        }

        public override void VisitDerefArray(ASTDereferenceArray n)
        {

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
                ReportError(n.Location, "Bounds of an array must be integers.");
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
        /// 
        /// Cflat also allows for declarations and assignments on the same line, so this will process the InitialValue sub tree
        /// as well.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitDeclLocal(ASTDeclarationLocal n)
        {
            if (!_scopeMgr.HasSymbol(n.ID))
            {
                CFlatType lhs = CheckSubTree(n.Type);
                if (lhs is TypeName)
                    lhs = ((TypeName)lhs).Base; // get the real type that the name represents

                //Check if the code is also assigning a value on the same line
                bool valid = true;

                if (n.InitialValue != null)
                {
                    CFlatType rhs = CheckSubTree(n.InitialValue);
                    if (!rhs.IsSupertype(lhs))
                    {
                        valid = false;
                        ReportError(n.Location, "Invalid assignment, type mismatch. Expected: {0} Got: {1}", TypeToFriendlyName(lhs), TypeToFriendlyName(rhs));
                    }
                }

                if (valid)
                    _scopeMgr.AddLocal(n.ID, lhs, _currentMethod);
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
            _currentClass = currentClass;
            
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

        private string TypeToFriendlyName(CFlatType t)
        {
            TypeClass c = t as TypeClass;
            return (c == null) ? t.ToString() : c.ClassName;
        }

        #endregion
    }
}
