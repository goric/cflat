﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeVoid : CFlatType
    {
        public override int Size
        {
            get
            {
                return 0;
            }
        }

        public override string ToString() { return "void"; }
    }
}
