using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeClass : CFlatType
    {
        public override bool IsClass { get { return true; } }
        //set this to true when instantiated?  not sure how to work this
        public override bool IsInstance { get { return true; } }
        public override CFlatType BaseType { get; set; }

        public string ClassName { get; set; }
        public ClassDescriptor Parent { get; set; }
        public Scope Scope { get; set; }

        public Dictionary<String, CFlatType> _fields;
        public Dictionary<String, CFlatType> _methods;

        public TypeClass(string name, ClassDescriptor parent = null)
        {
            ClassName = name;
            Parent = parent;

            _fields = new Dictionary<string,CFlatType>();
            _methods = new Dictionary<string,CFlatType>();
        }

        public void AddField (string name, CFlatType type)
        {
            _fields.Add(name, type);
        }

        public void AddMethod (string name, CFlatType type)
        {
            _methods.Add(name, type);
        }

        public override string ToString ()
        {
            return "class";
        }
    }
}
