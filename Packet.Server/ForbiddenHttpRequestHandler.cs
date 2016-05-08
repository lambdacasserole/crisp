﻿using System.Linq;
using System.Text.RegularExpressions;

using Packet.Interfaces.Configuration;
using Packet.Interfaces.Server;

namespace Packet.Server
{
    /// <summary>
    /// Represents a HTTP handler that serves 403 forbidden responses for forbidden resources.
    /// </summary>
    public class ForbiddenHttpRequestHandler : HttpRequestHandler
    {
        private readonly IUrlResolver _urlResolver;

        private readonly IPacketConfiguration _packetConfiguration;

        /// <summary>
        /// Initializes a new instance of a HTTP handler that serves 403 forbidden responses for forbidden resources.
        /// </summary>
        /// <param name="urlResolver"></param>
        /// <param name="packetConfigurationProvider"></param>
        public ForbiddenHttpRequestHandler(
            IUrlResolver urlResolver, 
            IPacketConfigurationProvider packetConfigurationProvider)
        {
            _urlResolver = urlResolver;
            _packetConfiguration = packetConfigurationProvider.Get();
        }

        /// <summary>
        /// Returns true if the given path matches a 'do not serve' rule. Otherwise returns false.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns></returns>
        private bool IsForbiddenPath(string path)
        {
            return _packetConfiguration.DoNotServePatterns.Any(p => Regex.IsMatch(path, p));
        }

        protected override IHttpResponse AttemptHandle(IHttpRequest request)
        {
            var resolvedPath = _urlResolver.GetUrlPath(request.Url);

            // Defer if file is not forbidden.
            if (!IsForbiddenPath(resolvedPath))
            {
                return null;
            }
            
            return new ForbiddenHttpResponse(request.Version, resolvedPath);
        }
    }
}
