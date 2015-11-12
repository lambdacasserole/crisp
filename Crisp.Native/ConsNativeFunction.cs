﻿using Crisp.Core;
using Crisp.Core.Evaluation;

namespace Crisp.Native
{
    /// <summary>
    /// Represents the basic function to create a pair from two expressions.
    /// </summary>
    public class ConsNativeFunction : IFunction
    {
        public IEvaluator Host { get; set; }

        public string Name => "cons";

        public SymbolicExpression Apply(SymbolicExpression expression, Context context)
        {
            expression.ThrowIfNotList(Name); // Takes a list of arguments.

            var arguments = expression.AsPair().Expand();
            arguments.ThrowIfWrongLength(Name, 2); // Must have two arguments.

            var head = Host.Evaluate(arguments[0], context);
            var tail = Host.Evaluate(arguments[1], context);

            return new Pair(head, tail);
        }
    }
}
