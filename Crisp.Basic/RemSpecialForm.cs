﻿using System.Collections.Generic;
using System.Linq;
using Crisp.Enums;
using Crisp.Interfaces;
using Crisp.Interfaces.Evaluation;
using Crisp.Interfaces.Types;
using Crisp.Shared;
using Crisp.Types;

namespace Crisp.Basic
{
    /// <summary>
    /// A function that given two expressions, returns the modulus (also known as the remainder) of their numeric 
    /// values.
    /// </summary>
    public class RemSpecialForm : SpecialForm
    {
        public override IEnumerable<string> Names => new List<string> {"rem", "mod"};

        public override ISymbolicExpression Apply(ISymbolicExpression expression, IEvaluator evaluator)
        {
            expression.ThrowIfNotList(Name); // Takes a list of arguments.

            var arguments = expression.AsPair().Expand();
            arguments.ThrowIfWrongLength(Name, 2); // Must have two arguments.

            // Attempt to evaluate every argument to a number.
            var evaluated = arguments.Select(evaluator.Evaluate).ToArray();
            if (evaluated.Any(e => e.Type != SymbolicExpressionType.Numeric))
            {
                throw new FunctionApplicationException(
                    $"The arguments to the function '{Name}' must all evaluate to the numeric type.");
            }

            return new NumericAtom(evaluated[0].AsNumeric().Value % evaluated[1].AsNumeric().Value);
        }
    }
}
