﻿using System.Collections.Generic;

using Crisp.Enums;
using Crisp.Interfaces.Evaluation;
using Crisp.Interfaces.Types;
using Crisp.Types;

namespace Crisp.Basic
{
    /// <summary>
    /// A special form that given an expression whose value is a pair, returns the pair's second value.
    /// </summary>
    public class CdrSpecialForm : SpecialForm
    {
        public override IEnumerable<string> Names => new List<string> {"cdr", "tail", "rest"};

        public override ISymbolicExpression Apply(ISymbolicExpression expression, IEvaluator evaluator)
        {
            expression.ThrowIfNotList(Name); // Takes a list of arguments.

            var arguments = expression.AsPair().Expand();
            arguments.ThrowIfWrongLength(Name, 1); // Must have one argument.

            // Result of evaluation of argument must be a pair.
            var evaluated = evaluator.Evaluate(arguments[0]);
            if (evaluated.Type != SymbolicExpressionType.Pair)
            {
                throw new FunctionApplicationException($"The argument to the function {Name} must be a pair.");
            }

            return evaluated.AsPair().Tail;
        }
    }
}
