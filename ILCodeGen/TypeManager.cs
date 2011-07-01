using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbstractSyntaxTree;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen
{
    public class TypeManager
    {
        public Dictionary<string, TypeBuilderInfo> TypeBuilderMap { get; private set; }

        public ModuleBuilder Module { get; private set; }

        public TypeManager(ModuleBuilder module)
        {
            TypeBuilderMap = new Dictionary<string, TypeBuilderInfo>();
            Module = module;
        }

        public void AddClass(ASTClassDefinition n)
        {
            TypeBuilderMap.Add(n.Name, new TypeBuilderInfo(n, Module));
        }

        public void AddSubClass(ASTSubClassDefinition n)
        {
            var parent = TypeBuilderMap[n.Parent];
            TypeBuilderMap.Add(n.Name, new TypeBuilderInfo(n, Module, parent));
        }

        public void AddMethod(string typeName, ASTDeclarationMethod n)
        {
            TypeBuilderInfo info = TypeBuilderMap[typeName];

            //we need to know the CIL type for the return type and arguments
            Type returnType = LookupCilType(n.ReturnType);
            TypeFunction function = n.Type as TypeFunction;

            MethodBuilder methodBuilder = info.Builder.DefineMethod(n.Name,
                MethodAccessibility(n), 
                returnType,
                ArgumentTypes(function));

            //store this MethodBuilder, keyed off its name
            info.MethodMap.Add(n.Name, new MethodBuilderInfo(methodBuilder, BuildFormalMap(n.Descriptor.Formals)));
        }

        public void AddField(string typeName, ASTDeclarationField n)
        {
            TypeBuilderInfo info = TypeBuilderMap[typeName];

            //define the field in the type builder, and save the FieldBuiler in a map, keyed off the name of the field
            FieldBuilder fieldBuilder = info.Builder.DefineField(n.Name, LookupCilType(n.Type), FieldAttributes.Public);
            info.FieldMap.Add(n.Name, fieldBuilder);
        }

        public void AddCtor(string typeName, ASTDeclarationCtor n)
        {
            TypeBuilderInfo info = TypeBuilderMap[typeName];
            TypeFunction function = n.Type as TypeFunction;

            ConstructorBuilder builderObj = info.Builder.DefineConstructor(MethodAttributes.Public,
                CallingConventions.Standard,
                ArgumentTypes(function));

            info.ConstructorBuilder = new ConstructorBuilderInfo(builderObj, BuildFormalMap(n.Descriptor.Formals));
        }

        public TypeBuilderInfo GetBuilderInfo(string typeName)
        {
            return TypeBuilderMap[typeName];
        }

        public MethodBuilderInfo GetMethodBuilderInfo(string typeName, string methodName)
        {
            return TypeBuilderMap[typeName].MethodMap[methodName];
        }

        /// <summary>
        /// Calls CreateType() on all TypeBuilders.
        /// </summary>
        public void CreateAllTypes()
        {
            foreach (TypeBuilderInfo info in TypeBuilderMap.Values)
                info.Builder.CreateType();
        }

        public Type LookupCilType(ASTType n)
        {
            return LookupCilType(n.Type);
        }

        public Type LookupCilType(CFlatType type)
        {
            if (type is TypeClass)
            {
                string name = (type as TypeClass).ClassName;
                return TypeBuilderMap[name].Builder;
            }
            else
                return type.CilType;
        }

        private Type[] ArgumentTypes(TypeFunction f)
        {
            return f.Formals.Values.Select(c => LookupCilType(c)).ToArray();
        }

        private MethodAttributes MethodAccessibility(ASTDeclarationMethod n)
        {
            //todo: this is a hack
            if (n.Name == CodeGenerator.MainMethodName)
                return MethodAttributes.Public | MethodAttributes.Static;
            else
                return MethodAttributes.Public;
        }

        private Dictionary<string, ArgumentInfo> BuildFormalMap(IEnumerable<FormalDescriptor> formals)
        {
            Dictionary<string, ArgumentInfo> map = new Dictionary<string, ArgumentInfo>();
            foreach (FormalDescriptor f in formals)
            {
                ArgumentInfo info = new ArgumentInfo(f.Name, LookupCilType(f.Type), map.Count);
                map.Add(f.Name, info);
            }
            return map;
        }
    }
}
