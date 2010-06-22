using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CFlat.SemanticPasses
{
    public class SecondPass : FirstPass
    {
        private ActualBuilder _actuals;
        private FormalBuilder _formals;

        public SecondPass()
            : base()
        {

        }
    }
}
