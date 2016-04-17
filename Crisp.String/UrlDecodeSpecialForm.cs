﻿using System.Linq;
using System.Web;

using Crisp.Shared;
using Crisp.Types;

namespace Crisp.String
{
    /*
     * Note: This function should be implemented in Crisp as it's not necessary to implement it at this level.
     */

    /// <summary>
    /// URL-decodes a string atom and returns the result.
    /// </summary>
    public class UrlDecodeSpecialForm : SpecialForm // TODO: Implement in Crisp.
    {
        public override string Name => "url-decode";

        public override ISymbolicExpression Apply(ISymbolicExpression expression, IEvaluator evaluator)
        {
            expression.ThrowIfNotList(Name); // Takes a list of arguments.

            var arguments = expression.AsPair().Expand();
            arguments.ThrowIfWrongLength(Name, 1); // Must have one argument.

            // Attempt to evaluate every argument to a string.
            var evaluated = arguments.Select(evaluator.Evaluate).ToArray();
            if (evaluated.Any(e => e.Type != SymbolicExpressionType.String))
            {
                throw new FunctionApplicationException(
                    $"The arguments to the function '{Name}' must all evaluate to the string type.");
            }

            return new StringAtom(HttpUtility.UrlDecode(evaluated[0].AsString().Value));
        }
    }
}
