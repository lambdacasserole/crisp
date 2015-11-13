﻿using Crisp.Core.Evaluation;

namespace Crisp.Core
{
    /// <summary>
    /// Represents a function that can be applied to an expression.
    /// </summary>
    public abstract class Function : SymbolicExpression
    {
        public override bool IsAtomic => false;

        public override SymbolicExpressionType Type => SymbolicExpressionType.Function;
        
        public abstract bool EvaluateArguments { get; }

        /// <summary>
        /// Applies the function to an expression.
        /// </summary>
        /// <param name="expression">The expression to apply the function to.</param>
        /// <param name="evaluator">The evaluator to use to evaluate the expression.</param>
        /// <returns></returns>
        public abstract SymbolicExpression Apply(SymbolicExpression expression, IEvaluator evaluator);
    }
}
