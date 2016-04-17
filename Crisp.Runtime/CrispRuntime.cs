﻿using Crisp.Evaluation;
using Crisp.Shared;

namespace Crisp.Runtime
{
    public class CrispRuntime : ICrispRuntime
    {
        private readonly IExpressionTreeSource _expressionTreeSource;
        private readonly IEvaluator _evaluator;

        public CrispRuntime(
            IExpressionTreeSource expressionTreeSource,
            IEvaluatorFactory evaluatorFactory)
        {
            _expressionTreeSource = expressionTreeSource;
            _evaluator = evaluatorFactory.Get();
        }

        public ISymbolicExpression Run()
        {
            return _evaluator.Evaluate(_expressionTreeSource.Get());
        }
    }
}
