﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Crisp.Core;

namespace Crisp.Native
{
    public class AddNativeFunction : INativeFunction
    {
        public INativeFunctionHost Host { get; set; }

        public string Name
        {
            get
            {
                return "add";
            }
        }

        public SymbolicExpression Apply(SymbolicExpression input)
        {
            var leftExpression = Host.Evaluate(input.LeftExpression).AsInteger();
             
            var rightExpression = Host.Evaluate(input.RightExpression.LeftExpression).AsInteger();

            return new SymbolicExpression(leftExpression + rightExpression);
        }
    }
}
