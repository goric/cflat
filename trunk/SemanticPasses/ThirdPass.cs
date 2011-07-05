using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;

using QUT.Gppg;

namespace CFlat.SemanticPasses
{
    public class ThirdPass : SecondPass, ICompilerPass
    {
        private TypeFunction _currentMethod;
        private bool _skipNextBlockScope;

        public ThirdPass(ASTNode treeNode, ScopeManager mgr)
            : base(treeNode, mgr)
        {

        }

        new public string PassName()
        {
            return "Third Semantic Pass";
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
        /// Used as the return value for methods that are type void, and have a return statement.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitVoidExpr(ASTVoidExpression n)
        {
            n.CFlatType = _lastSeenType = new TypeVoid();
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

        public override void VisitChar(ASTChar n)
        {
            n.CFlatType = _lastSeenType = new TypeChar();
        }

        #endregion

        #region Binary Operators

        public override void VisitGreater(ASTGreater n)
        {
            TypeCheckNumericBinary(n, "Operands for > must both be numeric.", new TypeBool());
        }

        public override void VisitGreaterEqual(ASTGreaterEqual n)
        {
            TypeCheckNumericBinary(n, "Operands for >= must both be numeric.", new TypeBool());
        }

        public override void VisitSmaller(ASTSmaller n)
        {
            TypeCheckNumericBinary(n, "Operands for < must both be numeric.", new TypeBool());
        }

        public override void VisitSmallerEqual(ASTSmallerEqual n)
        {
            TypeCheckNumericBinary(n, "Operands for <= must both be numeric.", new TypeBool());
        }

        public override void VisitMul(ASTMultiply n)
        {
            TypeCheckNumericBinary(n, "Operands for multiplication must both be numeric.");
        }

        public override void VisitDiv(ASTDivide n)
        {
            TypeCheckNumericBinary(n, "Operands for division must both be numeric.");
        }

        public override void VisitMod(ASTModulo n)
        {
            TypeCheckNumericBinary(n, "Operands for modulo must both be numeric.");
        }

        /// <summary>
        /// Binary operators are not supported for classes, only primatives.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitAdd(ASTAdd n)
        {
            CFlatType lhs = CheckSubTree(n.Left);
            CFlatType rhs = CheckSubTree(n.Right);

            if (lhs.IsNumeric && rhs.IsNumeric)
            {
                _lastSeenType = n.CFlatType = Supertype(lhs, rhs);
            }
            else
            {
                ReportError(n.Location, "Invalid operands for addition. Got types '{0}' and '{1}'.", TypeToFriendlyName(lhs), TypeToFriendlyName(rhs));
            }
        }

        public override void VisitConcatenate (ASTConcatenate n)
        {
            CFlatType lhs = CheckSubTree(n.Left);
            CFlatType rhs = CheckSubTree(n.Right);

            // we'll allow for implicit conversion from numeric types to strings for concat purposes
            if ((lhs.IsConcatenatable) && (rhs.IsConcatenatable))
                _lastSeenType = n.CFlatType = new TypeString();
            else
                ReportError(n.Location, "Operands for concatenation must be strings, characters, or integers.");
        }

        public override void VisitSub(ASTSubtract n)
        {
            TypeCheckNumericBinary(n, "Operands for subtraction must both be numeric.");
        }

        public override void VisitExponent(ASTExponent n)
        {
            TypeCheckNumericBinary(n, "Operands for exponentiation must both be numeric.");
        }

        public override void VisitAnd(ASTAnd n)
        {
            TypeCheckBooleanOperator(n, "Operands for && must both be booleans.");
        }

        public override void VisitOr(ASTOr n)
        {
            TypeCheckBooleanOperator(n, "Operands for || must both be booleans.");
        }

        public override void VisitEqual(ASTEqual n)
        {
            TypeCheckEquality(n);
        }

        public override void VisitNotEqual(ASTNotEqual n)
        {
            TypeCheckEquality(n);
        }

        #endregion

        #region Unary Operations

        /// <summary>
        /// These guys are a little trickier, can't just increment anything that typechecks to a numeric type
        /// </summary>
        /// <param name="n"></param>
        public override void VisitIncrement(ASTIncrement n)
        {
            //the second parameter, the string is simply for display purposes, I'm not doing any logic with it, cause that would be hella stupid
            _lastSeenType = n.CFlatType = TypeCheckIncrementDecrement(n.Expression, "++", n.Location);
        }

        public override void VisitDecrement(ASTDecrement n)
        {
            _lastSeenType = n.CFlatType = TypeCheckIncrementDecrement(n.Expression, "--", n.Location);
        }

        public override void VisitNeg(ASTNegative n)
        {
            CFlatType t = CheckSubTree(n.Expression);
            if (t.IsNumeric)
            {
                n.CFlatType = t;
                _lastSeenType = t;
            }
            else
            {
                ReportError(n.Location, "Only numeric datatypes can be negated. Got type '{0}'", TypeToFriendlyName(t));
            }
        }

        public override void VisitNot(ASTNot n)
        {
            CFlatType t = CheckSubTree(n.Expression);
            if (t is TypeBool)
            {
                n.CFlatType = t;
                _lastSeenType = t;
            }
            else
            {
                ReportError(n.Location, "Operand for the not operator must be a boolean. Got type '{0}'", TypeToFriendlyName(t));
            }
        }

        #endregion

        #region Statements

        /// <summary>
        /// Makes sure the left hand side of the assignment is a supertype of the right hand side.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitAssign(ASTAssign n)
        {
            n.LValue.IsLeftHandSide = true; //this is needed when we generate the IL.
            CFlatType lhs = CheckSubTree(n.LValue);
            CFlatType rhs = CheckSubTree(n.Expr);

            if (IsValidAssignment(lhs, rhs))
            {
                //check if the assignment is to a readonly method parameter
                CheckForReadonlyAssignments(n);

                //I believe we don't really do anything when the source code is correct in this case.
                n.CFlatType = new TypeVoid();
                _lastSeenType = n.CFlatType;
            }
            else
            {
                ReportError(n.Location, "Invalid assignment, type mismatch. Expected: {0} Got: {1}", TypeToFriendlyName(lhs), TypeToFriendlyName(rhs));
            }
        }

        private void CheckForReadonlyAssignments (ASTAssign n)
        {
            if (n.LValue.Descriptor is FormalDescriptor)
            {
                FormalDescriptor descriptor = (FormalDescriptor)n.LValue.Descriptor;
                if (String.Compare(descriptor.Modifier, READONLY_MODIFIER, true) == 0)
                {
                    ReportError(n.Location, "Parameter '{0}' is marked as readonly and can not be assigned.", descriptor.Name);
                }
            }

            bool isDerefField = (n.LValue is ASTDereferenceField && ((ASTDereferenceField)n.LValue).Object is ASTIdentifier);
            bool isDerefArray = (n.LValue is ASTDereferenceArray && ((ASTDereferenceArray)n.LValue).Array is ASTIdentifier);

            if (isDerefArray || isDerefField)
            {
                var item = isDerefField ? ((ASTDereferenceField)n.LValue).Object : ((ASTDereferenceArray)n.LValue).Array;
                var id = ((ASTIdentifier)item).ID;
                var curMethDesc = _scopeMgr.Find(_currentMethod.Name, d => d is MethodDescriptor) as MethodDescriptor;
                var formalMatch = curMethDesc.Formals.Where(p => p.Name.Equals(id, StringComparison.OrdinalIgnoreCase)).SingleOrDefault();

                if (formalMatch != null)
                {
                    if (!string.IsNullOrEmpty(formalMatch.Modifier)
                        && formalMatch.Modifier.Equals(READONLY_MODIFIER, StringComparison.OrdinalIgnoreCase))
                        ReportError(n.Location, "Parameter '{0}' passed to method '{1}' is marked as readonly and cannot be assigned.", formalMatch.Name, _currentMethod.Name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        public override void VisitFor(ASTFor n)
        {
            //define the block scope now to incapsulate the declared variable.
            BeginForLoopBlock();

            CFlatType initType = CheckSubTree(n.InitialExpr);
            n.InitialExpr.CFlatType = initType;

            TypeBool condition = CheckSubTree(n.Conditional) as TypeBool;
            if (condition != null)
            {
                n.Conditional.CFlatType = condition;
                CFlatType loopType = CheckSubTree(n.LoopExpr);
                n.LoopExpr.CFlatType = loopType;

                CheckSubTree(n.Body);
            }
            else
            {
                ReportError(n.Location, "The condition of a for loop must be a boolean.");
            }
        }

        public override void VisitForIn(ASTForIn n)
        {
            //define the block scope now to incapsulate the declared variable.
            BeginForLoopBlock();

            n.Lower.CFlatType = CheckSubTree(n.Lower);
            n.Upper.CFlatType = CheckSubTree(n.Upper);

            if (n.Lower.CFlatType is TypeInt && n.Upper.CFlatType is TypeInt)
            {
                //check if the temp variable has been assigned.
                if (!_scopeMgr.HasSymbol(n.TempVariable.ID))
                {
                    //we're good
                    _scopeMgr.AddLocal(n.TempVariable.ID, new TypeInt(), _currentMethod);

                    CheckSubTree(n.Body);
                }
                else
                {
                    ReportError(n.Location, "ID '{0}' has already been assigned.", n.TempVariable.ID);
                }
            }
            else
            {
                ReportError(n.Location, "Bounds of the for in loop must be integers. Got types '{0}' and '{1}'.", TypeToFriendlyName(n.Lower.CFlatType), TypeToFriendlyName(n.Upper.CFlatType));
            }
        }

        public override void VisitReturn(ASTReturn n)
        {
            CFlatType actual = CheckSubTree(n.ReturnValue);
            CFlatType expected = _currentMethod.ReturnType;
            if (actual.IsSupertype(expected))
            {
                n.CFlatType = expected;
                _lastSeenType = expected;

                _currentMethod.RegisterReturnStatement();
            }
            else
            {
                ReportError(n.Location, "Type mismatch in return statement. Expected: {0} Got: {1}", TypeToFriendlyName(expected), TypeToFriendlyName(actual));
            }
        }

        /// <summary>
        /// Open a new scope for the block and visit the body. Pop the scope when we're done.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitBlock(ASTBlock n)
        {
            //a hack to get around scoping declared variables properly in for loops
            if (!_skipNextBlockScope)
            {
                _scopeMgr.PushScope("block");
                _currentMethod.AddBlock(n.IsBranch);
            }
            else
            {
                _skipNextBlockScope = false;
            }

            CheckSubTree(n.Body);

            _currentMethod.LeaveBlock();
            _scopeMgr.PopScope();
        }

        /// <summary>
        /// Pretty simple, just ensure that the condition is a boolean.
        /// </summary>
        /// <param name="n"></param>
        public override void VisitWhile(ASTWhile n)
        {
            CFlatType condType = CheckSubTree(n.Condition);
            if (condType is TypeBool)
            {
                CheckSubTree(n.Body);
            }
            else
            {
                ReportError(n.Location, "While loop condition must be a boolean. Got type '{0}'", TypeToFriendlyName(condType));
            }
        }

        public override void VisitIfThen(ASTIfThen n)
        {
            CFlatType condType = CheckSubTree(n.Condition);
            if (condType is TypeBool)
            {
                CheckSubTree(n.Then);
            }
            else
            {
                ReportError(n.Location, "If statement must evaluate to boolean. Got type '{0}'", TypeToFriendlyName(condType));
            }
        }

        public override void VisitIfThenElse(ASTIfThenElse n)
        {
            CFlatType condType = CheckSubTree(n.Condition);
            if (condType is TypeBool)
            {
                CheckSubTree(n.Then);
                CheckSubTree(n.Else);
            }
            else
            {
                ReportError(n.Location, "If statement must evaluate to boolean. Got type '{0}'", TypeToFriendlyName(condType));
            }
        }

        public override void VisitStatementExpr(ASTStatementExpr n)
        {
            CheckSubTree(n.Expression);
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
                var cls = (ClassDescriptor)_scopeMgr.Find(n.ClassName, p => p is ClassDescriptor);
                var func = cls.Methods.Where(prop => prop.Name.Equals(n.ClassName, StringComparison.OrdinalIgnoreCase)).SingleOrDefault().Type as TypeFunction;
                //Check if the class we're working with has a constructor of the same name
                if (func != null)
                {
                    CheckSubTree(n.Actuals);
                    //check the method signature of the constructor to make sure the correct arguments are passed in
                    ActualBuilder builder = new ActualBuilder();
                    n.Actuals.Visit(builder);

                    if (func.AcceptCall(builder.Actuals))
                    {
                        //hooray, the code is valid
                        MethodDescriptor ctorDescriptor = (MethodDescriptor)_scopeMgr.Find(n.ClassName, p => p is MethodDescriptor, func.Scope);
                        _lastSeenType = desc.Type;
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
                var descriptor = (ClassDescriptor)_scopeMgr.Find(lvalue.ClassName, p => p is ClassDescriptor);
                //check if a method with the given name exists in the scope.
                //This needs to check not only the class's shallow scope, but all the parents as well.
                MethodDescriptor methodDesc = _scopeMgr.Find(n.Method, d => d is MethodDescriptor, descriptor.Scope) as MethodDescriptor;
                
                //if (methodDesc != null && (descriptor.Methods.Contains(methodDesc) || methodDesc.IsCFlatMethod))
                if (methodDesc != null)
                {
                    if (methodDesc.Modifiers.Contains(PRIVATE_MODIFIER, StringComparer.InvariantCultureIgnoreCase))
                        if (!_scopeMgr.HasSymbol(n.Method, _currentClass.Scope))
                            ReportError(n.Location, "Cannot access the private method {0} in class {1} from class {2}", methodDesc.Name, methodDesc.ContainingClass.Name, _currentClass.ClassName);
                    
                    //check if the arguments match
                    TypeFunction method = (TypeFunction)methodDesc.Type;
                    //visit any actuals that need processing
                    CheckSubTree(n.Actuals);
                    //collect the actuals
                    var curMethDesc = _scopeMgr.Find(_currentMethod.Name, d => d is MethodDescriptor) as MethodDescriptor;
                    ActualBuilder builder = new ActualBuilder(curMethDesc);
                    n.Actuals.Visit(builder);

                    if (method.AcceptCall(builder.Actuals)) //if the types check
                    {
                        CheckActualsHaveReadonly(methodDesc, builder, n);

                        CheckReadonlyNotPassedAsModifiable(methodDesc, builder, n);
                        
                        n.Descriptor = methodDesc;
                        n.CFlatType = method.ReturnType;

                        _lastSeenType = method.ReturnType;
                    }
                    else
                    {
                        ReportError(n.Location, "Invalid parameters for method '{0}.{1}'", TypeToFriendlyName(lvalue), n.Method);
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

        public void CheckActualsHaveReadonly (MethodDescriptor desc, ActualBuilder builder, ASTInvoke n)
        {
            //check that for all readonly formals the actual is being passes with a readonly modifier
            int index = 0;
            foreach (var formal in desc.Formals)
            {
                if (!String.IsNullOrEmpty(formal.Modifier)
                        && formal.Modifier.Equals(READONLY_MODIFIER, StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrEmpty(builder.Actuals[index].Modifier)
                        || !builder.Actuals[index].Modifier.Equals(READONLY_MODIFIER, StringComparison.OrdinalIgnoreCase))
                        ReportError(n.Location, "Missing readonly declaration on method invocation - Parameter '{0}' on function '{1}' must be marked as readonly.", formal.Name, desc.Name);
                }
                index++;
            }
        }

        public void CheckReadonlyNotPassedAsModifiable (MethodDescriptor methodDesc, ActualBuilder builder, ASTInvoke n)
        {
            //check for readonly params being passed as modifiable parameters.
            int index = 0;
            foreach (var actual in builder.Actuals)
            {
                if (actual.IsFromFormal && actual.IsFormalReadonly)
                {
                    if (String.IsNullOrEmpty(methodDesc.Formals[index].Modifier)
                        || !methodDesc.Formals[index].Modifier.Equals(READONLY_MODIFIER, StringComparison.OrdinalIgnoreCase))
                        ReportError(n.Location, "Cannot pass readonly identifier '{0}' as a modifiable parameter to method '{1}'", actual.Name, methodDesc.Name);
                }
                index++;
            }
        }

        public override void VisitSelf(ASTSelf n)
        {
            n.CFlatType = _currentClass;
            n.Descriptor = _scopeMgr.Find(_currentClass.ClassName, d => d is ClassDescriptor);

            _lastSeenType = _currentClass;
        }

        public override void VisitBase(ASTBase n)
        {
            if (_currentClass.Parent != null)
            {
                n.CFlatType = _currentClass.Parent.Type;
                _lastSeenType = _currentClass.Parent.Type;
            }
            else
            {
                ReportError(n.Location, "Class '{0}' does not have a parent class.", _currentClass.ClassName);
            }
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
                var descriptor = (ClassDescriptor)_scopeMgr.Find(lvalue.ClassName, p => p is ClassDescriptor);
                //check if a field exists.
                MemberDescriptor memberDesc = _scopeMgr.Find(n.Field, d => d is MemberDescriptor, descriptor.Scope) as MemberDescriptor;

                if (memberDesc != null && descriptor.Fields.Contains(memberDesc))
                {
                    if (memberDesc.Modifiers.Contains(PRIVATE_MODIFIER, StringComparer.InvariantCultureIgnoreCase))
                        if (!_scopeMgr.HasSymbolShallow(n.Field))
                            ReportError(n.Location, "Cannot reference the private member '{0}' on class '{1}' from class '{2}'", memberDesc.Name, memberDesc.ContainingClass.Name, _currentClass.ClassName);

                    if (memberDesc != null)
                    {
                        //hooray, code is valid
                        n.Descriptor = memberDesc;
                        n.CFlatType = memberDesc.Type;

                        _lastSeenType = memberDesc.Type;
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
            CFlatType arrType = CheckSubTree(n.Array);
            CFlatType index = CheckSubTree(n.Index);
            if (arrType.IsArray && index is TypeInt)
            {
                _lastSeenType = n.CFlatType = ((TypeArray)arrType).BaseType;

            }
            else
            {
                ReportError(n.Location, "Array index must be an integer.");
            }
        }

        /// <summary>
        /// Checks to make sure that both the upper and lower bounds are integers
        /// </summary>
        /// <param name="n"></param>
        public override void VisitInstantiateArray(ASTInstantiateArray n)
        {
            n.Lower.CFlatType = CheckSubTree(n.Lower);
            n.Upper.CFlatType = CheckSubTree(n.Upper);
            
            if ((n.Lower.CFlatType is TypeInt) && (n.Upper.CFlatType is TypeInt))
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
            //if the method is tagged as readonly, this translates to all of its formals being readonly
            var func = _scopeMgr.Find(n.Name, p => p is MethodDescriptor) as MethodDescriptor;
            if (func.Modifiers.Contains("readonly", StringComparer.OrdinalIgnoreCase))
                foreach (var formal in func.Formals)
                    formal.Modifier = "readonly";

            VisitMethodBody(string.Format("body {0}", n.Name), n.Body, n.Type as TypeFunction);

           
            if ((_currentMethod.ReturnType is TypeVoid == false) && (!_currentMethod.AllCodePathsReturn()))
            {
                ReportError(n.Location, "Not all code paths of method '{0}' return a value.", n.Name);
            }
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

                //Check if the code is also assigning a value on the same line
                bool valid = true;

                if (n.InitialValue != null)
                {
                    CFlatType rhs = CheckSubTree(n.InitialValue);
                    if (rhs is TypeFunction && ((TypeFunction)rhs).IsConstructor)
                        rhs = new TypeClass(((TypeFunction)rhs).Name);
                    if (!IsValidAssignment(lhs, rhs))
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

        private CFlatType TypeCheckIncrementDecrement(ASTExpression expr, string operatorName, LexLocation loc)
        {
            //totally cheating here. the grammar should not even allow this to happen.
            if (expr is ASTIdentifier)
            {
                ASTIdentifier identifier = (ASTIdentifier)expr;
                //need to set a flag so that the code generator can store back the result
                identifier.IsLeftHandSide = true;

                CFlatType t = CheckSubTree(identifier);
                if (t.IsNumeric)
                {
                    return t;
                }
                else
                {
                    ReportError(loc, "The {0} operator requires an instance of a numeric datatype", operatorName);
                }
            }
            else
            {
                ReportError(loc, "The {0} operator requires an instance of a numeric datatype", operatorName);
            }

            /* note: the ReportError method will always throw, so this part of the code should not be reached,
             * unless of course we change the ReportError method to not throw or try to implement some error recovery
             * strategy...
             * */
            throw new InternalCompilerException("This part of the code should not be reachable.");
        }

        /// <summary>
        /// This method looks kinda ugly because I'm using it for all binary operations, which may actually typecheck
        /// to different types
        /// </summary>
        /// <param name="n"></param>
        /// <param name="errorMessage"></param>
        /// <param name="resultingType">The type that the expression will evaluate as. This is optional, so if you
        /// don't pass it in, the compiler will use the supertype of the two operands.
        /// </param>
        private void TypeCheckNumericBinary(ASTBinary n, string errorMessage, CFlatType resultingType = null)
        {
            bool valid = false;
            CFlatType lhs = CheckSubTree(n.Left);
            if (lhs.IsNumeric)
            {
                CFlatType rhs = CheckSubTree(n.Right);
                if (rhs.IsNumeric)
                {
                    //if the parameter was not passed in, we'll figure it out
                    resultingType = resultingType ?? Supertype(lhs, rhs);

                    n.CFlatType = resultingType;
                    _lastSeenType = resultingType;
                    valid = true;
                }
            }

            if (!valid)
            {
                ReportError(n.Location, errorMessage);
            }
        }

        private void TypeCheckBooleanOperator(ASTBinary n, string errorMessage)
        {
            bool valid = false;
            CFlatType lhs = CheckSubTree(n.Left);
            if (lhs is TypeBool)
            {
                CFlatType rhs = CheckSubTree(n.Right);
                if (rhs is TypeBool)
                {
                    _lastSeenType = n.CFlatType = new TypeBool();
                    valid = true;
                }
            }

            if (!valid)
            {
                ReportError(n.Location, errorMessage);
            }
        }

        /// <summary>
        /// A hack to scope the declared variable in a for loop to inside the block of the for loop properly.
        /// </summary>
        private void BeginForLoopBlock()
        {
            _skipNextBlockScope = true;
            _scopeMgr.PushScope("block");
            _currentMethod.AddBlock(false);
        }

        private void TypeCheckEquality(ASTBinary n)
        {
            CFlatType lhs = CheckSubTree(n.Left);
            CFlatType rhs = CheckSubTree(n.Right);

            if (lhs.IsSupertype(rhs) || rhs.IsSupertype(lhs))
            {
                //we're good
                _lastSeenType = n.CFlatType = new TypeBool();
            }
            else
            {
                //invalid
                ReportError(n.Location, "Types '{0}' and '{1}' are not compatible for equality.", TypeToFriendlyName(lhs), TypeToFriendlyName(rhs));
            }
        }

        /// <summary>
        /// This kind of logic should probably be in the ToString() methods of TypeClass
        /// and TypeArray...
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private string TypeToFriendlyName(CFlatType t)
        {
            if (t.IsClass)
                return ((TypeClass)t).ClassName;
            else if (t.IsArray)
            {
                TypeArray arr = (TypeArray)t;
                string baseType = TypeToFriendlyName(arr.BaseType);
                return String.Concat(baseType, "[]");
            }
            else
                return t.ToString();
        }

        /// <summary>
        /// Takes two types and returns which one is the super class of the two.
        /// If both types are the same, then of course the first is returned.
        /// If the types are totally unrelated (that is, neither is a supertype), then null is returned (or maybe it should throw?)
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        private CFlatType Supertype(CFlatType t1, CFlatType t2)
        {
            if (t1.IsSupertype(t2))
                return t2;
            else if (t2.IsSupertype(t1))
                return t1;
            else
                return null;
        }

        /// <summary>
        /// Returns if the assignment is valid, i.e. if the expression:
        /// lhs = rhs;
        /// can be evaluated.
        /// 
        /// Returns true if both sides are numeric, of if the left hand side is a super type of the right hand side.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        private bool IsValidAssignment(CFlatType lhs, CFlatType rhs)
        {
            return ((lhs.IsNumeric && rhs.IsNumeric) || rhs.IsSupertype(lhs));
        }

        #endregion
    }
}
