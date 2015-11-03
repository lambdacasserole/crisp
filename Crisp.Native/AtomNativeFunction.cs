﻿using Crisp.Core;

using System.Linq;

namespace Crisp.Native
{
    /// <summary>
    /// Represents the basic atomic test function.
    /// </summary>
    public class AtomNativeFunction : IFunction
    {
        public IFunctionHost Host { get; set; }

        public string Name => "atom";

        public SymbolicExpression Apply(SymbolicExpression expression, Context context)
        {
            expression.ThrowIfNotList(Name); // Takes a list of arguments.

            var arguments = expression.AsPair().Expand();
            arguments.ThrowIfWrongLength(Name, 1); // Must have one argument.

            var evaluated = Host.Evaluate(arguments[0], context);

            return evaluated.IsAtomic ? SymbolAtom.True : SymbolAtom.False;
        }
    }
}
