﻿namespace Crisp.Shared
{
    public interface ICrispRuntime
    {
        ISymbolicExpression Run(IExpressionTreeSource argumentSource);
    }
}