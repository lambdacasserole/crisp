﻿using System.Collections.Generic;
using System.IO;

using Crisp.Enums;
using Crisp.Interfaces.Evaluation;
using Crisp.Interfaces.Types;
using Crisp.IoC;
using Crisp.Types;

namespace Crisp.Basic
{
    public class RequireSpecialForm : SpecialForm
    {
        private readonly Dictionary<string, Dictionary<string, ISymbolicExpression>> _cachedBindings;

        public override IEnumerable<string> Names => new List<string> {"require", "import"};

        public RequireSpecialForm()
        {
            // Cache loaded/parsed files.
            _cachedBindings = new Dictionary<string, Dictionary<string, ISymbolicExpression>>();
        }

        public override ISymbolicExpression Apply(ISymbolicExpression expression, IEvaluator evaluator)
        {
            expression.ThrowIfNotList(Name); // Takes a list of arguments.

            var arguments = expression.AsPair().Expand();
            arguments.ThrowIfWrongLength(Name, 2); // Must have two arguments.

            // First argument must be a string.
            var pathExpr = evaluator.Evaluate(arguments[0]);
            if (pathExpr.Type != SymbolicExpressionType.String)
            {
                throw new FunctionApplicationException($"The first argument to the {Name} function must be a" +
                                                       " string atom.");
            }

            // Calculate file path.
            var rawPath = pathExpr.AsString().Value;
            string path;
            if (rawPath.StartsWith("~"))
            {
                path = rawPath.Replace("~", evaluator.InterpreterDirectory);
            }
            else
            {
                path = Path.IsPathRooted(rawPath) ? rawPath : Path.Combine(evaluator.SourceFileDirectory, rawPath);
            }


            // Check file exists.
            if (!File.Exists(path))
            {
                throw new FunctionApplicationException($"The path specified for the {Name} function specifies a" +
                                                       " file that does not exist.");
            }

            // Read file if not already cached.
            if (!_cachedBindings.ContainsKey(path))
            {
                var fileText = $"({File.ReadAllText(path)})";
                var libraryExpressionTree = CrispRuntimeFactory.SourceToExpressionTree(fileText);
                var bindingPairs = libraryExpressionTree.Get().AsPair().Expand();
                var newBindings = new Dictionary<string, ISymbolicExpression>();
                foreach (var binding in bindingPairs)
                {
                    var pair = binding.AsPair();
                    newBindings.Add(pair.Head.AsSymbol().Value, pair.Tail);
                }
                _cachedBindings.Add(path, newBindings);
            }

            // Change source file directory for binding.
            var oldSourceFileDirectory = evaluator.SourceFileDirectory;
            var bindingEvaluator = evaluator.Derive();
            bindingEvaluator.SourceFileDirectory = Path.GetDirectoryName(path);
            bindingEvaluator.Mutate(_cachedBindings[path]);

            // Revert to original source file directory for evaluation.
            var newEvaluator = bindingEvaluator.Derive();
            newEvaluator.SourceFileDirectory = oldSourceFileDirectory; 

            return newEvaluator.Evaluate(arguments[1]);
        }
    }
}
