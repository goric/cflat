using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    public class FirstPass : Visitor
    {
        public bool Failed { get; protected set; }
        private ASTNode _treeNode;
        protected ScopeManager _scopeMgr;

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
        /// Visit a class node and add the descriptor to the current scope
        /// </summary>
        /// <param name="n"></param>
        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            TypeClass cls = new TypeClass() { ClassName = n.Name };
            n.ClassDesc = _scopeMgr.AddClass(cls.ClassName, cls, null);
        }


        /// <summary>
        /// Visit subclass node, adding appropriate descriptors to the current scope
        /// </summary>
        /// <param name="n"></param>
        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            TypeClass cls = new TypeClass() { ClassName = n.Name };
            ClassDescriptor prnt = _scopeMgr.GetType(n.Parent);
            //prnt should be null if it's not a type or isn't found.. check for error, then check the actual type
            if (prnt == null || !prnt.IsType) { Failed = true; Console.WriteLine("Could not find base type " + n.Parent + " for type " + n.Name); } 
            cls.BaseType = prnt.Type;
            if (cls.BaseType == null || !cls.IsClass) { Failed = true; Console.WriteLine("Could not find base type " + n.Parent + " for type " + n.Name); } 
            n.Descriptor = _scopeMgr.AddClass(cls.ClassName, cls, prnt);        
        }
    }
}
