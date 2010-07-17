using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AbstractSyntaxTree;

namespace CFlat
{
    public static class ExtensionMethods
    {
        public static bool IsAnyType(this object o, params Type[] args)
        {
            Type objType = o.GetType();
            foreach (Type t in args)
            {
                if (objType == t)
                    return true;
            }
            return false;
        }
    }
}
