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
        public Scope TopScope { get; private set; }

        public ScopeManager()
        {
            //top level scope
            CurrentScope = new Scope("top", null);
            TopScope = CurrentScope;
        }

        public Scope PushScope(string name)
        {
            CurrentScope = new Scope(name, CurrentScope);
            return CurrentScope;
        }
        public Scope PopScope()
        {
            var old = CurrentScope;
            CurrentScope = CurrentScope.Parent;
            return old;
        }

        public void RestoreScope(Scope s)
        {
            CurrentScope = s;
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

        public MethodDescriptor AddMethod(string name, CFlatType type, TypeClass containingClass)
        {
            var md = new MethodDescriptor(type);
            CurrentScope.Descriptors.Add(name, md);
            containingClass.AddMethod(name, type);
            return md;
        }

        public FormalDescriptor AddFormal(string name, CFlatType type, string modifier)
        {
            var descriptor = new FormalDescriptor(type, name, modifier);
            CurrentScope.Descriptors.Add(name, descriptor);
            return descriptor;
        }

        public MemberDescriptor AddMember(string name, CFlatType type, TypeClass containingClass)
        {
            var descriptor = new MemberDescriptor(type);
            CurrentScope.Descriptors.Add(name, descriptor);
            containingClass.AddField(name, type);
            return descriptor;
        }

        public LocalDescriptor AddLocal(string name, CFlatType type, TypeFunction containingMethod)
        {
            var descriptior = new LocalDescriptor(type, name);
            CurrentScope.Descriptors.Add(name, descriptior);
            containingMethod.AddLocal(name, type);
            return descriptior;
        }

        #region Scope Traversal

        /// <summary>
        /// Finds the given symbol in the current scope.
        /// Specify a predicate to find only Types, Methods, etc.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pred"></param>
        /// <returns>A Descriptor if found, null otherwise.</returns>
        public Descriptor Find(string name, Func<Descriptor, bool> pred)
        {
            return Find(name, pred, CurrentScope);
        }

        /// <summary>
        /// Finds the given symbol in the given scope.
        /// Specify a predicate to find only Types, Methods, etc.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pred"></param>
        /// <param name="s"></param>
        /// <returns>A Descriptor if found, null otherwise.</returns>
        public Descriptor Find(string name, Func<Descriptor, bool> pred, Scope s)
        {
            Scope checkScope = s;

            while (checkScope != null)
            {
                if (checkScope.HasSymbol(name))
                {
                    Descriptor d = checkScope.Descriptors[name];
                    if (pred(d))
                        return d;
                    //This thing could actually short circuit and return false here, cause we're looking in a dictionary.
                    //I'll hold off on doing that cause maybe we want to change the data structure to a bucket to allow overloading and shadowing. The performance penalty for looping is pretty much nothing so w/e.
                }

                checkScope = checkScope.Parent;
            }

            return null;
        }

        /// <summary>
        /// Need to go up scope hierarchy and see if this is indeed a type
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Descriptor GetType(string name)
        {
            //refactored the scope traversal out into 1 method.
            return Find(name, d => d.IsType);
        }

        public bool HasSymbol(string identifier)
        {
            return HasSymbol(identifier, CurrentScope);
        }

        public bool HasSymbol(string identifier, Scope s)
        {
            return Find(identifier, d => true,  s) != null;
        }

        public bool HasSymbolShallow(string identifier)
        {
            return CurrentScope.HasSymbol(identifier);
        }

        #endregion
    }
}
