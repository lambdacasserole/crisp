﻿using System.Collections.Generic;

using Crisp.Enums;
using Crisp.Interfaces.Evaluation;
using Crisp.Interfaces.Types;
using Crisp.Types;

namespace Crisp.Basic
{
    /// <summary>
    /// A special form that given two expressions, returns true if the value of the first is numerically less than or  
    /// equal to the value of the second; false if not.
    /// </summary>
    public class LeqSpecialForm : SpecialForm
    {
        public override IEnumerable<string> Names => new List<string> {"leq"};

        public override ISymbolicExpression Apply(ISymbolicExpression expression, IEvaluator evaluator)
        {
            expression.ThrowIfNotList(Name); // Takes a list of arguments.

            var arguments = expression.AsPair().Expand();
            arguments.ThrowIfWrongLength(Name, 2); // Must have two arguments.

            // Evaluate both parameters.
            var x = evaluator.Evaluate(arguments[0]);
            var y = evaluator.Evaluate(arguments[1]);
            
            var t = new BooleanAtom(true);
            var f = new BooleanAtom(false);

            // Different types can never be equal.
            if (x.Type != y.Type)
            {
                return f;
            }

            switch (x.Type)
            {
                case SymbolicExpressionType.Boolean:
                case SymbolicExpressionType.Constant:
                case SymbolicExpressionType.Symbol:
                case SymbolicExpressionType.Nil:
                case SymbolicExpressionType.String:
                    return x.Equals(y) ? t : f;
                case SymbolicExpressionType.Numeric:
                    return x.AsNumeric().Value <= y.AsNumeric().Value ? t : f; // Less than only relevant for numeric atoms.
                default:
                    return f;
            }
        }
    }
}
