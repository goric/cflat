using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.SemanticPasses
{
    public interface ICompilerPass
    {
        void Run();
        string PassName();
    }
}
