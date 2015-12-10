﻿using System.IO;

using Crisp.Core.Preprocessing;

using Packet.Configuration;

namespace Packet
{
    /// <summary>
    /// An implementation of a service that returns the fully-qualified directory path of the directory in which 
    /// compiled special form binaries are stored.
    /// </summary>
    internal class SpecialFormDirectoryPathProvider : ISpecialFormDirectoryPathProvider
    {
        private readonly IConfigurationProvider _configurationProvider;

        private readonly IInterpreterDirectoryPathProvider _interpreterDirectoryPathProvider;

        public string GetPath()
        {
            return Path.Combine(_interpreterDirectoryPathProvider.GetPath(),
                _configurationProvider.GetConfiguration().SpecialFormDirectory);
        }

        /// <summary>
        /// Initializes a new instance of a special form directory provider.
        /// </summary>
        /// <param name="configurationProvider">The service to use to get the application configuration.</param>
        /// <param name="interpreterDirectoryPathProvider">The service to use to get the interpreter path.</param>
        public SpecialFormDirectoryPathProvider(IConfigurationProvider configurationProvider,
            IInterpreterDirectoryPathProvider interpreterDirectoryPathProvider)
        {
            _configurationProvider = configurationProvider;
            _interpreterDirectoryPathProvider = interpreterDirectoryPathProvider;
        }
    }
}
