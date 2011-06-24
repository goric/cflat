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
        public string Name { get; private set; }

        public Block BodyBlock { get; set; }
        public Block CurrentBlock { get; set; }

        public Dictionary<string, CFlatType> Formals;
        public Dictionary<string, CFlatType> Locals;

        public TypeFunction(string name)
            : this(false)
        {
            Name = name;
        }

        public TypeFunction(bool isCtor)
        {
            IsConstructor = isCtor;
            Formals = new Dictionary<string, CFlatType>();
            Locals = new Dictionary<string, CFlatType>();

            BodyBlock = new Block(false);
            CurrentBlock = BodyBlock;
        }

        public void AddFormal(string name, CFlatType type)
        {
            Formals.Add(name, type);
        }

        public void AddLocal(string name, CFlatType type)
        {
            CurrentBlock.AddLocal(name, type);
        }

        public void AddBlock(bool branchStatement)
        {
            Block b = new Block(branchStatement);
            CurrentBlock.AddBlock(b);
            CurrentBlock = b;
        }

        public void LeaveBlock()
        {
            CurrentBlock = CurrentBlock.Parent;
        }
        
        public bool HasLocal(string name)
        {
            Block tempBlock = CurrentBlock;
            do
            {
                if (tempBlock.HasLocal(name))
                    return true;
                else
                    tempBlock = tempBlock.Parent;
            }
            while (tempBlock != null);

            return false;
        }

        public void RegisterReturnStatement()
        {
            CurrentBlock.HasReturnStatement = true;
        }

        public bool AllCodePathsReturn()
        {
            return BodyBlock.AllCodePathsReturn();
        }

        /// <summary>
        /// Checks if the given list of actuals satisfied the type constraints of this method.
        /// Checks to make sure there are the same number of actuals and formals, and that each formal is a
        /// supertype of the given actual
        /// </summary>
        /// <param name="actuals"></param>
        /// <returns></returns>
        public bool AcceptCall(List<ActualDescriptor> actuals)
        {
            List<CFlatType> formals = Formals.Values.OfType<CFlatType>().ToList();
            if (formals.Count != actuals.Count)
                return false;

            for (int i = 0; i < formals.Count; i++)
            {
                //is the formal a super type of what we're passing in? If not, then this is not valid
                if (!formals[i].IsSupertype(actuals[i].Type))
                    return false;
            }

            return true;
        }

        public override bool IsSupertype(TypeFunction checkType)
        {
            throw new NotImplementedException();
        }


        public override string ToString() { return ""; }

        public override Type CilType
        {
            get { throw new NotImplementedException(); }
        }
    }
}
