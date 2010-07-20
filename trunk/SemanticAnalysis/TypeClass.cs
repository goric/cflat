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

        public string ClassName { get; set; }
        public ClassDescriptor Parent { get; set; }
        public Scope Scope { get; set; }

        public Dictionary<String, CFlatType> Fields;
        public Dictionary<String, CFlatType> Methods;

        public TypeClass(string name, ClassDescriptor parent = null)
        {
            ClassName = name;
            Parent = parent;

            Fields = new Dictionary<string,CFlatType>();
            Methods = new Dictionary<string,CFlatType>();
        }

        public void AddField (string name, CFlatType type)
        {
            Fields.Add(name, type);
        }

        public void AddMethod (string name, CFlatType type)
        {
            Methods.Add(name, type);
        }

        public override string ToString ()
        {
            return "class";
        }

        /// <summary>
        /// returns true if checkType is a super type of this
        /// </summary>
        /// <param name="checkType"></param>
        /// <returns></returns>
        public override bool IsSupertype(TypeClass checkType)
        {
            if (checkType.ClassName == this.ClassName)
                return true;
            else if (this.Parent != null)
                return this.Parent.Type.IsSupertype(checkType);
            else
                return false;
        }
    }
}
