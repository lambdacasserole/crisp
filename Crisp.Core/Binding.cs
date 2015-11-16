﻿using Crisp.Core.Evaluation;

namespace Crisp.Core
{
    /// <summary>
    /// Represents a binding between a symbol and an expression.
    /// </summary>
    public class Binding
    {
        /// <summary>
        /// The evaluator to use when the bound expression is lazily evaluated
        /// </summary>
        private readonly IEvaluator _evaluator;

        /// <summary>
        /// The original, unevaluated bound expression.
        /// </summary>
        private readonly SymbolicExpression _expression;

        /// <summary>
        /// Holds the expression once it has been lazily evaluated.
        /// </summary>
        private SymbolicExpression _evaluated;

        /// <summary>
        /// The symbol that is bound to the expression.
        /// </summary>
        public SymbolAtom Symbol { get; private set; }

        /// <summary>
        /// The expression that is bound to the symbol.
        /// </summary>
        public SymbolicExpression Value => _evaluated ?? (_evaluated = _evaluator.Evaluate(_expression));
        
        /// <summary>
        /// Initializes a new instance of a binding between a symbol and an expression.
        /// </summary>
        /// <param name="symbol">The symbol that is bound to the expression.</param>
        /// <param name="expression">The expression that is bound to the symbol.</param>
        /// <param name="evaluator">The evaluator to use when the bound expression is lazily evaluated.</param>
        public Binding(SymbolAtom symbol, SymbolicExpression expression, IEvaluator evaluator)
        {
            Symbol = symbol;
            _expression = expression;
            _evaluator = evaluator;
        }
    }
}
