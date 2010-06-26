using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SemanticAnalysis;

namespace CFlat.SemanticPasses
{
    public class ScopeManager
    {
        public Scope CurrentScope { get; set; }

        public ScopeManager()
        {
            //top level scope
            CurrentScope = new Scope("top", null);
        }

        /// <summary>
        /// create a descriptor and add it to the current scope
        /// </summary>
        /// <param name="name"></param>
        /// <param name="t"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public ClassDescriptor AddClass(string name, CFlatType t, ClassDescriptor parent)
        {
            ClassDescriptor cd = new ClassDescriptor(t, parent);
            CurrentScope.Descriptors.Add(name, cd);
            return cd;
        }

        /// <summary>
        /// Need to go up scope hierarchy and see if this is indeed a type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ClassDescriptor GetType(string name)
        {
            Scope checkScope = CurrentScope;

            while (checkScope != null)
            {
                //if this is a type and it's in this scope, return it
                if (checkScope.Descriptors.ContainsKey(name) && checkScope.Descriptors[name].IsType)
                {
                    return (ClassDescriptor)checkScope.Descriptors[name];
                }
                checkScope = checkScope.Parent;
            }

            return null;
        }

    }
}
