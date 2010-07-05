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

        public Scope PushScope (string name)
        {
            CurrentScope = new Scope(name, CurrentScope);
            return CurrentScope;
        }
        public Scope PopScope ()
        {
            var old = CurrentScope;
            CurrentScope = CurrentScope.Parent;
            return old;
        }

        /// <summary>
        /// create a descriptor and add it to the current scope
        /// </summary>
        /// <param name="name"></param>
        /// <param name="t"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public ClassDescriptor AddClass(string name, CFlatType t, ClassDescriptor parent = null)
        {
            ClassDescriptor cd = new ClassDescriptor(t, parent);
            CurrentScope.Descriptors.Add(name, cd);
            return cd;
        }

        public MethodDescriptor AddMethod (string name, CFlatType type, TypeClass containingClass)
        {
            var md = new MethodDescriptor(type);
            CurrentScope.Descriptors.Add(name, md);
            containingClass.AddMethod(name, type);
            return md;
        }

        public FormalDescriptor AddFormal (string name, CFlatType type)
        {
            var descriptor = new FormalDescriptor(type, name);
            CurrentScope.Descriptors.Add(name, descriptor);
            return descriptor;
        }

        public MemberDescriptor AddMember (string name, CFlatType type, TypeClass containingClass)
        {
            var descriptor = new MemberDescriptor(type);
            CurrentScope.Descriptors.Add(name, descriptor);
            containingClass.AddField(name, type);
            return descriptor;
        }

        /// <summary>
        /// Need to go up scope hierarchy and see if this is indeed a type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Descriptor GetType(string name)
        {
            Scope checkScope = CurrentScope;

            while (checkScope != null)
            {
                //if this is a type and it's in this scope, return it
                if (checkScope.Descriptors.ContainsKey(name) && checkScope.Descriptors[name].IsType)
                {
                    return (Descriptor)checkScope.Descriptors[name];
                }
                checkScope = checkScope.Parent;
            }

            return null;
        }

    }
}
