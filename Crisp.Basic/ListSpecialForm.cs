﻿using System.Collections.Generic;

using Crisp.Interfaces.Evaluation;
using Crisp.Interfaces.Types;
using Crisp.Types;

namespace Crisp.Basic
{
    /// <summary>
    /// A special form that returns its arguments as a proper list.
    /// </summary>
    public class ListSpecialForm : SpecialForm
    {
        public override IEnumerable<string> Names => new List<string> {"list"};

        public override ISymbolicExpression Apply(ISymbolicExpression expression, IEvaluator evaluator)
        {
            expression.ThrowIfNotList(Name); // Takes a list of arguments.
            return evaluator.Evaluate(expression.AsPair());
        }
    }
}
