using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;

namespace ILCodeGen
{
    public class TypeManager
    {
        public Dictionary<string, string> InheritanceMap;

        public Dictionary<string, List<ASTDeclarationMethod>> MethodMap;

        public Dictionary<string, System.Reflection.Emit.TypeBuilder> CFlatTypes;

        public TypeManager()
        {
            InheritanceMap = new Dictionary<string, string>();
            MethodMap = new Dictionary<string, List<ASTDeclarationMethod>>();
            CFlatTypes = new Dictionary<string, System.Reflection.Emit.TypeBuilder>();
        }
    }
}
