﻿using System.Collections.Generic;
using System.IO;
using Packet.Interfaces.Configuration;
using Packet.Interfaces.Logging;
using Packet.Interfaces.Server;

namespace Packet.Server
{
    public class StaticFileHttpRequestHandler : HttpRequestHandler
    {
        private readonly IPacketConfiguration _packetConfiguration;
        private readonly IUrlResolver _urlResolver;
        private readonly ILogger _logger;

        public StaticFileHttpRequestHandler(
            IPacketConfigurationProvider packetConfigurationProvider, 
            IUrlResolver urlResolver, 
            ILogger logger) 
        {
            _packetConfiguration = packetConfigurationProvider.Get();
            _urlResolver = urlResolver;
            _logger = logger;
        }

        /// <summary>
        /// Gets the MIME type for the given file extension.
        /// </summary>
        /// <param name="extension">The file extension to return the MIME type for.</param>
        /// <returns></returns>
        private string GetMimeTypeForExtension(string extension)
        {
            string value;
            return _packetConfiguration.MimeTypeMappings.TryGetValue(extension, out value) ?
                value : "application/octet-stream"; // Default to this MIME type.
        }

        protected override IHttpResponse AttemptHandle(IHttpRequest request)
        {
            var resolvedPath = _urlResolver.Resolve(request.Url);

            return new FullHttpResponse(request.Version)
            {
                StatusCode = 200,
                Headers = new Dictionary<string, string>
                {
                    {"Content-Type", GetMimeTypeForExtension(Path.GetExtension(resolvedPath))}
                },
                Content = File.ReadAllBytes(resolvedPath)
            };
        }
    }
}
