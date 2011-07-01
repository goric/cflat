using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace ILCodeGen
{
    public class LocalBuilderInfo
    {
        public LocalBuilderInfo(int index, string name, LocalBuilder builder)
        {
            Index = index;
            Name = name;
            Builder = builder;
        }

        public int Index { get; private set; }

        public string Name { get; private set; }

        public LocalBuilder Builder { get; private set; }
    }
}
