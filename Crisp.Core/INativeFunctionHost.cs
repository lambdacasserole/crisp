﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crisp.Core
{
    public interface INativeFunctionHost
    {
        SymbolicExpression Evaluate(SymbolicExpression expression);
    }
}
