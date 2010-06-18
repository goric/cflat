using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeClass : CFlatType
    {
        public override bool IsClass
        {
            get
            {
                return true;
            }
        }

        public override string ToString() { return "class"; }

        //set this to true when instantiated?  not sure how to work this
        public override bool IsInstance
        {
            get { return true; }
        }
    }
}
