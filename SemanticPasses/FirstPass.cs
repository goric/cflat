﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;
using ILCodeGen;
using ILCodeGen.SystemMethods;

using QUT.Gppg;

namespace CFlat.SemanticPasses
{
    /// <summary>
    /// First semantic pass finds/validates all classes and saves their important information
    /// in an instance of a ScopeManager.  The next passes will gather and analyze field/function
    /// information and finally local information.
    /// </summary>
    public class FirstPass : Visitor, ICompilerPass
    {
        private ASTNode _treeNode;
        protected ScopeManager _scopeMgr;
        protected TypeClass _currentClass;
        protected CFlatType _lastSeenType;

        private const string GlobalScopeName = "__global";

        public FirstPass(ASTNode treeNode, ScopeManager mgr)
        {
            _treeNode = treeNode;
            _scopeMgr = mgr;

            if (!_scopeMgr.CurrentScope.HasSymbol(GlobalScopeName))
            {
                TypeClass globalClass = new TypeClass(GlobalScopeName);
                globalClass.Descriptor = _scopeMgr.AddClass(globalClass.ClassName, globalClass, null);
                
                //setup built in system methods
                foreach (SystemMethod m in SystemMethodManager.Methods())
                {
                    m.FuncInfo.Scope = _scopeMgr.TopScope;
                    _scopeMgr.AddMethod(m.Name, m.FuncInfo, globalClass, null, true);
                }
            }
        }

        public void Run() 
        {
            _treeNode.Visit(this);    
        }

        public string PassName()
        {
            return "First Semantic Pass";
        }

        /// <summary>
        /// Added a single function incase we want to do something besides writing to the console.
        /// 
        /// Changed this to throw an exception so our compiler will just stop at the first error it finds.
        /// Attempting to continue on error will end up with whacky stuff happening in ThirdPass (NREs) and would make our code
        /// a LOT more complex
        /// </summary>
        public void ReportError(LexLocation loc, string msg, params string[] formatArgs)
        {
            string formattedMsg = String.Format(msg, formatArgs);
            var location = string.Format(" line {0} column {1}", loc.StartLine, loc.StartColumn);
            throw new SourceCodeErrorException(String.Format(
                "{0}{1}  at {2}", 
                formattedMsg, 
                Environment.NewLine, 
                (loc != null) ? location : "unknown")
                );
        }

        /// <summary>
        /// Visiting an abstract node means something is wrong, so we'll error
        /// </summary>
        /// <param name="n"></param>
        public override void VisitNode(ASTNode n)
        {
            throw new InternalCompilerException("VisitNode was called");
        }

        /// <summary>
        /// Walk the subtree under the passed in node, by visiting the root of the subtree
        /// which should visit its children and so on.  Returns the type of the tree root.
        /// </summary>
        public CFlatType CheckSubTree (ASTNode root)
        {
            _lastSeenType = null;
            root.Visit(this);
            return _lastSeenType;
        }
        
        /// <summary>
        /// Visit a class node and add the descriptor to the current scope
        /// </summary>
        /// <param name="n"></param>
        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            CheckForGlobalScope(n.Name, n.Location);

            TypeClass cls = new TypeClass(n.Name);
            _currentClass = cls;
            n.Descriptor = _scopeMgr.AddClass(cls.ClassName, cls, null);
            n.Type= cls;
        }


        /// <summary>
        /// Visit subclass node, adding appropriate descriptors to the current scope
        /// </summary>
        /// <param name="n"></param>
        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            CheckForGlobalScope(n.Name, n.Location);

            ClassDescriptor prnt = (ClassDescriptor)_scopeMgr.GetType(n.Parent);
            
            TypeClass cls = new TypeClass(n.Name, prnt);
            _currentClass = cls;

            //prnt should be null if it's not a type or isn't found.. check for error, then check the actual type
            if (prnt == null || !prnt.IsType) { ReportError(n.Location, "Could not find base type '{0}' for type '{1}'.", n.Parent, n.Name); } 
            if (!cls.IsClass) { ReportError(n.Location, "Could not find base type '{0}' for type '{1}'.", n.Parent, n.Name); } 
            n.Descriptor = _scopeMgr.AddClass(cls.ClassName, cls, prnt);
            n.Type = cls;
        }
        
        /// <summary>
        /// Checks if someone is redefining the global scope, which results in a compiler error.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="loc"></param>
        private void CheckForGlobalScope(string name, LexLocation loc)
        {
            if (name == GlobalScopeName)
                ReportError(loc, "The class name {0} is reserved for internal compiler use.", GlobalScopeName);
        }
    }
}
