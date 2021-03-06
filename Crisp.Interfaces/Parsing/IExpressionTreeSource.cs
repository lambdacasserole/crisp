﻿using Crisp.Interfaces.Types;

namespace Crisp.Interfaces.Parsing
{
    /// <summary>
    /// Represents an expression tree source.
    /// </summary>
    public interface IExpressionTreeSource
    {
        /// <summary>
        /// Gets the expression tree from this source.
        /// </summary>
        /// <returns></returns>
        ISymbolicExpression Get();
    }
}
