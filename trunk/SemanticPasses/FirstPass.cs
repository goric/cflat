using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    /// <summary>
    /// First semantic pass finds/validates all classes and saves their important information
    /// in an instance of a ScopeManager.  The next passes will gather and analyze field/function
    /// information and finally local information.
    /// </summary>
    public class FirstPass : Visitor
    {
        public bool Failed { get; protected set; }
        private ASTNode _treeNode;
        protected ScopeManager _scopeMgr;
        protected TypeClass _currentClass;
        protected CFlatType _lastSeenType;

        public FirstPass(ASTNode treeNode)
        {
            Failed = false;
            _treeNode = treeNode;
            _scopeMgr = new ScopeManager();
        }

        public void Run() 
        {
            _treeNode.Visit(this);    
        }

        /// <summary>
        /// Visiting an abstract node means something is wrong, so we'll error
        /// </summary>
        /// <param name="n"></param>
        public override void VisitNode(ASTNode n)
        {
            Failed = true;
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
            ClassDescriptor prnt = (ClassDescriptor)_scopeMgr.GetType(n.Parent);
            TypeClass cls = new TypeClass(n.Name, prnt);
            _currentClass = cls;

            //prnt should be null if it's not a type or isn't found.. check for error, then check the actual type
            if (prnt == null || !prnt.IsType) { Failed = true; Console.WriteLine("Could not find base type " + n.Parent + " for type " + n.Name); } 
            //cls.BaseType = prnt.Type;
            if (cls.BaseType == null || !cls.IsClass) { Failed = true; Console.WriteLine("Could not find base type " + n.Parent + " for type " + n.Name); } 
            n.Descriptor = _scopeMgr.AddClass(cls.ClassName, cls, prnt);
            n.Type = cls;
        }
    }
}
