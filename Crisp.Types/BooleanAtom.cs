﻿using Crisp.Enums;

namespace Crisp.Types
{
    /// <summary>
    /// Represents an boolean string expression.
    /// </summary>
    public sealed class BooleanAtom : Atom<bool>
    {
        public override SymbolicExpressionType Type => SymbolicExpressionType.Boolean;

        public override bool Value { get; protected set; }

        /// <summary>
        /// Initializes a new instance of a boolean atomic expression.
        /// </summary>
        /// <param name="value">The value of the expression.</param>
        public BooleanAtom(bool value)
        {
            Value = value;
        }
    }
}
