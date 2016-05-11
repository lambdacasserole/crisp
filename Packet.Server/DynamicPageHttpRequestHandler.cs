﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Crisp.Enums;
using Crisp.Interfaces.Types;
using Crisp.IoC;
using Crisp.Types;

using Packet.Interfaces.Configuration;
using Packet.Interfaces.Logging;
using Packet.Interfaces.Server;

namespace Packet.Server
{
    /// <summary>
    /// Represents a HTTP request handler for serving dynamic pages.
    /// </summary>
    public class DynamicPageHttpRequestHandler : HttpRequestHandler
    {
        private readonly IPacketConfiguration _packetConfiguration;

        private readonly IUrlResolver _urlResolver;

        private readonly IErrorPageContentRetriever _errorPageContentRetriever;

        private readonly Encoding _encoding;

        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of a HTTP request handler for serving dynamic pages.
        /// </summary>
        /// <param name="packetConfigurationProvider">The server configuration provider service.</param>
        /// <param name="urlResolver">The URL resolution service.</param>
        /// <param name="errorPageContentRetriever">The error page content retrieval service.</param>
        /// <param name="encodingProvider">The encoding provider service.</param>
        /// <param name="logger">The logger to use to log server events.</param>
        public DynamicPageHttpRequestHandler(
            IPacketConfigurationProvider packetConfigurationProvider,
            IUrlResolver urlResolver,
            IErrorPageContentRetriever errorPageContentRetriever,
            IEncodingProvider encodingProvider,
            ILogger logger)
        {
            _packetConfiguration = packetConfigurationProvider.Get(); 
            _urlResolver = urlResolver;
            _errorPageContentRetriever = errorPageContentRetriever;
            _encoding = encodingProvider.Get();
            _logger = logger;
        }

        /// <summary>
        /// Returns true if the given file extension if configured to be interpreted.
        /// </summary>
        /// <param name="extension">The file extension to check, including the leading dot.</param>
        /// <returns></returns>
        private bool IsInterpretedFileExtension(string extension)
        {
            return _packetConfiguration.CrispFileExtensions.Contains(extension);
        }

        /// <summary>
        /// Converts a name-value collection of HTTP headers passed back by a Crisp webpage to a dictionary.
        /// </summary>
        /// <param name="headers">The symbolic expression containingthe name-value collection to convert.</param>
        /// <returns></returns>
        private static Dictionary<string, string> TransformHeadersForPacket(ISymbolicExpression headers)
        {
            if (headers.Type == SymbolicExpressionType.Nil)
            {
                return new Dictionary<string, string>();
            }

            var expanded = headers.AsPair().Expand();
            return expanded.ToDictionary(p => p.AsPair().Head.AsString().Value, p => p.AsPair().Tail.AsString().Value);
        }

        protected override IHttpResponse AttemptHandle(IHttpRequest request)
        {
            var resolvedPath = _urlResolver.Resolve(request.Url); // Resolve URL.

            // If not dynamic page, defer.
            if (!IsInterpretedFileExtension(Path.GetExtension(resolvedPath)))
            {
                return null;
            }
            
            // Evaluate webpage.
            IList<ISymbolicExpression> result;
            try
            {
                // Create runtime for file and run it.
                var runtime = CrispRuntimeFactory.GetCrispRuntime(resolvedPath);
                var rawResult = runtime.Run(new HttpExpressionTreeSource(request));

                // Program response should be a list.
                if (rawResult.Type != SymbolicExpressionType.Pair)
                {
                    throw new ApplicationException("Program result was not a list.");
                }

                result = rawResult.AsPair().Expand(); // Expand list.

                // Check types in list.
                if (result[0].Type != SymbolicExpressionType.String
                    || result[1].Type != SymbolicExpressionType.Numeric
                    || result[2].Type != SymbolicExpressionType.String
                    || (result[3].Type != SymbolicExpressionType.Pair && result[3].Type != SymbolicExpressionType.Nil))
                {
                    throw new ApplicationException("Program result was not in a valid format.");
                }
            }
            catch (Exception ex)
            {
                _logger.WriteLine($"Error: {ex.Message}");

                // Return 500 internal server error.
                return new FullHttpResponse(request.Version)
                {
                    StatusCode = 500,
                    Headers = new Dictionary<string, string>
                    {
                        {"Content-Type", "text/html"}
                    },
                    Content = _errorPageContentRetriever.GetEncodedErrorPageContent(500)
                };
            }

            // Transform headers.
            var headers = TransformHeadersForPacket(result[3]);
            if (!headers.ContainsKey("Content-Type"))
            {
                headers.Add("Content-Type", result[2].AsString().Value);
            }
            
            var content = _encoding.GetBytes(result[0].AsString().Value); // Encode content.

            // Simple request means simple response.
            if (request.Version.Major == 0 && request.Version.Minor == 9)
            {
                return new SimpleHttpResponse(content);
            }

            // Full request means full response.
            return new FullHttpResponse(request.Version)
            {
                StatusCode = Convert.ToInt32(result[1].AsNumeric().Value),
                Content = content,
                Headers = headers
            };
        }
    }
}
