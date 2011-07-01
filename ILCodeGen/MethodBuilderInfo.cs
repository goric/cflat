using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using SemanticAnalysis;

namespace ILCodeGen
{
    public class MethodBuilderInfo : BuilderInfo
    {
        /// <summary>
        /// Used to declare a Method with no parameters
        /// </summary>
        /// <param name="builder"></param>
        public MethodBuilderInfo(MethodBuilder builder) 
            : this(builder, null)
        {

        }

        /// <summary>
        /// Used to declare a Method with a list of parameters.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="formals"></param>
        public MethodBuilderInfo(MethodBuilder builder, Dictionary<string, ArgumentInfo> formals)
            : base(builder, formals)
        {
            
        }

        public MethodBuilder Builder
        {
            get { return Method as MethodBuilder; }
        }
    }
}
