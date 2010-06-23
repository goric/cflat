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

        public FirstPass(ASTNode treeNode)
        {
            Failed = false;
            _treeNode = treeNode;
        }

        public void Run() 
        {
            _treeNode.Visit(this);    
        }

        
        public override void VisitClassDefinition(ASTClassDefinition n)
        {
            TypeClass cls = new TypeClass() { ClassName = n.Name };
            n.ClassDesc = new ClassDescriptor(cls, null);
        }


        public override void VisitSubClassDefinition(ASTSubClassDefinition n)
        {
            TypeClass prnt = new TypeClass() { ClassName = n.Parent };
            TypeClass chld = new TypeClass() { ClassName = n.Name };
            n.Descriptor = new ClassDescriptor(chld, new ClassDescriptor(prnt, null));
        }
    }
}
