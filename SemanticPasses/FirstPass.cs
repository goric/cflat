﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.SemanticPasses
{
    public class FirstPass
    {
        public bool Failed { get; protected set; }

        public FirstPass()
        {
            Failed = false;
        }
    }
}
