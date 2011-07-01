using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen
{
    public abstract class BuilderInfo
    {
        /// <summary>
        /// Used to declare a Method with no parameters
        /// </summary>
        /// <param name="builder"></param>
        protected BuilderInfo(MethodBase builderObj)
            : this(builderObj, null)
        {

        }

        /// <summary>
        /// Used to declare a Method with a list of parameters.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="formals"></param>
        protected BuilderInfo(MethodBase builderObj, Dictionary<string, ArgumentInfo> formals)
        {
            Method = builderObj;
            Locals = new Dictionary<string, LocalBuilderInfo>();
            Arguments = formals ?? new Dictionary<string, ArgumentInfo>();
        }

        public string Name
        {
            get { return Method.Name; }
        }

        public virtual MethodBase Method { get; private set; }

        public Dictionary<string, LocalBuilderInfo> Locals { get; private set; }

        public Dictionary<string, ArgumentInfo> Arguments { get; private set; }

        public LocalBuilderInfo AddLocal(string name, LocalBuilder builder)
        {
            LocalBuilderInfo info = new LocalBuilderInfo(Locals.Count, name, builder);
            Locals.Add(name, info);
            return info;
        }
    }
}
