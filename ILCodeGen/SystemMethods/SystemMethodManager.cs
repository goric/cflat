using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SemanticAnalysis;
using System.Reflection;

namespace ILCodeGen.SystemMethods
{
    public static class SystemMethodManager
    {
        private static Dictionary<string, SystemMethod> _methods = new Dictionary<string, SystemMethod>();

        static SystemMethodManager()
        {
            Type target = typeof(SystemMethod);
            foreach (Type subType in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(target) && !t.IsAbstract))
            {
                SystemMethod instance = (SystemMethod)Activator.CreateInstance(subType);
                _methods.Add(instance.Name, instance);
            }
        }

        public static IEnumerable<SystemMethod> Methods()
        {
            return _methods.Values.Cast<SystemMethod>();
        }

        public static bool IsSystemMethod(string name)
        {
            return _methods.ContainsKey(name);
        }

        public static SystemMethod Lookup(string name)
        {
            return _methods[name];
        }
    }
}
