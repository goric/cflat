﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    /// <summary>
    /// Class for storing descriptors for formal parameters so actual parameters
    /// can be type checked against them
    /// </summary>
    public class FormalDescriptor : Descriptor
    {
        public string Name { get; private set; }

        /// <summary>
        /// Takes a type and the index in the function of a parameter
        /// </summary>
        public FormalDescriptor (CFlatType type, string name) : base(type)
        {
            Name = name;
        }
    }
}
