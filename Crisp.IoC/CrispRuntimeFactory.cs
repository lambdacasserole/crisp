﻿using SimpleInjector;

using Crisp.Configuration;
using Crisp.Evaluation;
using Crisp.Interfaces.Configuration;
using Crisp.Interfaces.Evaluation;
using Crisp.Interfaces.Parsing;
using Crisp.Interfaces.Runtime;
using Crisp.Interfaces.Shared;
using Crisp.Interfaces.Tokenization;
using Crisp.Shared;
using Crisp.Parsing;
using Crisp.Runtime;
using Crisp.Tokenization;

namespace Crisp.IoC
{
    /// <summary>
    /// Provides convenience methods for parsing and running Crisp code.
    /// </summary>
    public static class CrispRuntimeFactory
    {
        /// <summary>
        /// Creates a new <see cref="ICrispRuntime"/> instance to run the file at the specified path.
        /// </summary>
        /// <param name="filepath">The path of the input file to create the runtime for.</param>
        /// <returns></returns>
        public static ICrispRuntime GetCrispRuntime(string filepath)
        {
            // Dependency injection.
            var container = new Container();
            container.Register<ISourceFilePathProvider>(() => new SourceFilePathProvider(filepath));
            container.Register<ISourceFileDirectoryPathProvider, SourceFileDirectoryPathProvider>();
            container.Register<ISourceCodeProvider, FileSourceCodeProvider>();
            container.Register(TokenizerConfigurationProviderFactory.GetCrispTokenizerConfigurationProvider);
            container.Register(TokenFilterFactory.GetCommentAndWhitespaceFilter);
            container.Register<ITokenListSource, Tokenizer>();
            container.Register<IExpressionTreeSource, Parser>();
            container.Register<IInterpreterFilePathProvider, InterpreterFilePathProvider>();
            container.Register<IInterpreterDirectoryPathProvider, InterpreterDirectoryPathProvider>();
            container.Register<ICrispConfigurationFileNameProvider>(() => new CrispConfigurationFileNameProvider("crisp.json"));
            container.Register<IRawCrispConfigurationProvider, RawCrispConfigurationProvider>();
            container.Register<ICrispConfigurationProvider, CrispConfigurationProvider>();
            container.Register<ISpecialFormDirectoryPathProvider, SpecialFormDirectoryPathProvider>();
            container.Register<ISpecialFormLoader, CachingSpecialFormLoader>();
            container.Register<IEvaluatorFactory, EvaluatorFactory>();
            container.Register<ICrispRuntime, CrispRuntime>();
            container.Verify();

            return container.GetInstance<ICrispRuntime>(); // Return runtime.
        }

        /// <summary>
        /// Creates a new <see cref="IExpressionTreeSource"/> instance from the given source code.
        /// </summary>
        /// <param name="source">The source code to parse.</param>
        /// <returns></returns>
        public static IExpressionTreeSource SourceToExpressionTree(string source)
        {
            // Dependency injection.
            var container = new Container();
            container.Register<ISourceCodeProvider>(() => new SourceCodeProvider(source));
            container.Register(TokenizerConfigurationProviderFactory.GetCrispTokenizerConfigurationProvider);
            container.Register(TokenFilterFactory.GetCommentAndWhitespaceFilter);
            container.Register<ITokenListSource, Tokenizer>();
            container.Register<IExpressionTreeSource, Parser>();
            container.Verify();

            return container.GetInstance<IExpressionTreeSource>(); // Return parsed source.
        }
    }
}
