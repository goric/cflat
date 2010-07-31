using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemanticAnalysis
{
    public class Block
    {
        public Block(bool isBranch)
        {
            _childBlocks = new List<Block>();
            _locals = new Dictionary<string, CFlatType>();
            IsBranch = isBranch;
        }


        private List<Block> _childBlocks;
        private Dictionary<string, CFlatType> _locals;

        public bool IsBranch { get; set; }

        public Block Parent { get; set; }

        public bool HasReturnStatement { get; set; }

        public ICollection<Block> ChildBlocks()
        {
            return _childBlocks.AsReadOnly();
        }

        public void AddBlock(Block b)
        {
            b.Parent = this;
            _childBlocks.Add(b);
        }

        public void AddLocal(string name, CFlatType type)
        {
            _locals.Add(name, type);
        }

        public bool HasLocal(string name)
        {
            return _locals.ContainsKey(name);
        }

        /// <summary>
        /// All code paths return a value if the main block returns, or if all of the branching blocks
        /// all return.
        /// </summary>
        /// <returns></returns>
        public bool AllCodePathsReturn()
        {
            if (HasReturnStatement)
                return true;
            else
                return _childBlocks.Where(b => b.IsBranch).All(b => b.HasReturnStatement);
        }
    }
}
