﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

using Crisp.Core.Preprocessing;
using Crisp.Core.Types;

namespace Crisp.Core.Evaluation
{
    /// <summary>
    /// An implementation of an expression evaluator.
    /// </summary>
    public class Evaluator : IEvaluator
    {
        /// <summary>
        /// A list of bindings between symbols and expressions.
        /// </summary>
        private readonly List<Binding> _bindings;

        public string SourceFilePath { get; }

        public string SourceFolderPath { get; }

        /// <summary>
        /// Initializes a new instance of an expression evaluator.
        /// </summary>
        /// <param name="sourceFilePathProvider"></param>
        /// <param name="specialFormLoader"></param>
        /// <param name="dependencyLoader"></param>
        public Evaluator(
            ISourceFilePathProvider sourceFilePathProvider, 
            ISpecialFormLoader specialFormLoader, 
            IDependencyLoader dependencyLoader)
        {
            _bindings = new List<Binding>();
            SourceFilePath = sourceFilePathProvider.GetPath();
            SourceFolderPath = Path.GetDirectoryName(SourceFilePath);
            MutableBind(specialFormLoader.GetBindings());
            MutableBind(dependencyLoader.GetBindings());
        }

        /// <summary>
        /// Initializes a new instance of an expression evaluator.
        /// </summary>
        /// <param name="sourceFilePath"></param>
        /// <param name="sourceFolderPath"></param>
        /// <param name="bindings">A list of bindings from which to initialize the evaluator.</param>
        public Evaluator(string sourceFilePath, string sourceFolderPath, List<Binding> bindings)
        {
            _bindings = bindings;
            SourceFilePath = sourceFilePath;
            SourceFolderPath = sourceFolderPath;
        }

        public IEvaluator Bind(Dictionary<SymbolAtom, SymbolicExpression> bindings)
        {
            // We need an all-new list.
            var newBindings = new List<Binding>(_bindings);
            newBindings.AddRange(bindings.Select(b => new Binding(b.Key, b.Value, this)));

            return new Evaluator(SourceFilePath, SourceFolderPath, newBindings); // Return an all-new evaluator.
        }

        public IEvaluator Bind(SymbolAtom symbol, SymbolicExpression expression)
        {
            return Bind(new Dictionary<SymbolAtom, SymbolicExpression>()
            {
                {symbol, expression}
            });
        }

        public void MutableBind(Dictionary<SymbolAtom, SymbolicExpression> bindings)
        {
            _bindings.AddRange(bindings.Select(b => new Binding(b.Key, b.Value, this)));
        }

        public void MutableBind(SymbolAtom symbol, SymbolicExpression expression)
        {
            _bindings.Add(new Binding(symbol, expression, this));
        }

        /// <summary>
        /// Returns the binding for a given symbol.
        /// </summary>
        /// <param name="symbol">The symbol to return the binding for.</param>
        /// <returns></returns>
        private Binding Lookup(SymbolAtom symbol)
        {
            if (!IsBound(symbol))
            {
                throw new RuntimeException($"Use of name {symbol.Value} which is unbound or outside its scope.");
            }
            return _bindings.Last(b => b.Symbol.Matches(symbol));
        }

        /// <summary>
        /// Returns whether or not a binding currently exists for a given symbol.
        /// </summary>
        /// <param name="symbol">The symbol to check for.</param>
        /// <returns></returns>
        private bool IsBound(SymbolAtom symbol)
        {
            return _bindings.Any(b => b.Symbol.Matches(symbol));
        }

        public SymbolicExpression Evaluate(SymbolicExpression expression)
        {
            switch (expression.Type)
            {
                case SymbolicExpressionType.Symbol:
                    return Lookup(expression.AsSymbol()).Value;
                case SymbolicExpressionType.Pair:
                    var pair = expression.AsPair();

                    // Might pair be a function application?
                    if (pair.IsExplicitlyBracketed) 
                    {
                        // Evaluate head to get function.
                        var head = Evaluate(pair.Head); 
                        if (head.Type == SymbolicExpressionType.Function)
                        {
                            var function = head.AsFunction();
                            var args = function.SkipArgumentEvaluation ? pair.Tail : Evaluate(pair.Tail); // Don't evaluate arguments to special forms.
                            return function.Apply(args, this);
                        }  
                    }
                    return new Pair(Evaluate(pair.Head), Evaluate(pair.Tail)); // Evaluate pair.
                default:
                    return expression; // Non-symbol atoms evaluate to themselves.
            }
        }
    }
}
