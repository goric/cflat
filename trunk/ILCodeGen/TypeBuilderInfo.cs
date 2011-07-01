using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using System.Reflection;
using System.Reflection.Emit;

namespace ILCodeGen
{
    public class TypeBuilderInfo
    {
        public TypeBuilderInfo(ASTClassDefinition n, ModuleBuilder module)
        {
            Builder = module.DefineType(n.Name, TypeAttributes.Public);
            Init();
        }

        public TypeBuilderInfo(ASTSubClassDefinition n, ModuleBuilder module, TypeBuilderInfo parent)
        {
            Builder = module.DefineType(n.Name, TypeAttributes.Public, parent.Builder);
            Init();
        }

        private void Init()
        {
            MethodMap = new Dictionary<string, MethodBuilderInfo>();
            FieldMap = new Dictionary<string, FieldBuilder>();
        }

        public string Name
        {
            get { return Builder.Name; }
        }

        public TypeBuilder Builder { get; private set; }

        /// <summary>
        /// Since we're not supporting method overloading, we only have 1 constructor per type. Lame, huh?
        /// </summary>
        public ConstructorBuilderInfo ConstructorBuilder { get; set; }

        public Dictionary<string, MethodBuilderInfo> MethodMap { get; private set; }

        public Dictionary<string, FieldBuilder> FieldMap { get; private set; }
    }
}
