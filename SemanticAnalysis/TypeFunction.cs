using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class TypeFunction : CFlatType
    {
        public override bool IsFunction { get { return true; } }
        public bool IsConstructor { get; set; }
        public Scope Scope { get; set; }
        public CFlatType ReturnType { get; set; }

        public Dictionary<string, CFlatType> Formals;
        public Dictionary<string, CFlatType> Locals;

        public TypeFunction()
            : this(false)
        {

        }

        public TypeFunction(bool isCtor)
        {
            IsConstructor = isCtor;
            Formals = new Dictionary<string, CFlatType>();
            Locals = new Dictionary<string, CFlatType>();
        }

        public void AddFormal(string name, CFlatType type)
        {
            Formals.Add(name, type);
        }

        public void AddLocal(string name, CFlatType type)
        {
            Locals.Add(name, type);
        }

        /// <summary>
        /// Checks if the given list of actuals satisfied the type constraints of this method.
        /// Checks to make sure there are the same number of actuals and formals, and that each formal is a
        /// supertype of the given actual
        /// </summary>
        /// <param name="actuals"></param>
        /// <returns></returns>
        public bool AcceptCall(List<CFlatType> actuals)
        {
            List<CFlatType> formals = Formals.Values.OfType<CFlatType>().ToList();
            if (formals.Count != actuals.Count)
                return false;

            for (int i = 0; i < formals.Count; i++)
            {
                //is the formal a super type of what we're passing in? If not, then this is not valid
                if (!actuals[i].IsSupertype(formals[i]))
                    return false;
            }

            return true;
        }

        public override bool IsSupertype(TypeFunction checkType)
        {
            throw new NotImplementedException();
        }

        public override string ToString() { return ""; }
    }
}
