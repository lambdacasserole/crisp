﻿using System.IO;

namespace Crisp.Core.Preprocessing
{
    /// <summary>
    /// An implementation of a require path transformer.
    /// </summary>
    public class RequirePathTransformer : IRequirePathTransformer
    {
        private readonly IInterpreterDirectoryPathProvider _interpreterDirectoryPathProvider;
        
        public string Transform(string path)
        {
            // The tilde means relative to the interpreter executable.
            var filename = path.StartsWith("~/")
                ? _interpreterDirectoryPathProvider.GetPath() + path.TrimStart('~')
                : path;
            
            // Convert relative paths into absolute paths.
            var fileInfo = new FileInfo(filename);
            return Path.IsPathRooted(filename)
                ? filename
                : Path.Combine(fileInfo.DirectoryName ?? string.Empty, filename);
        }

        /// <summary>
        /// Initializes a new instance of a require path transformer.
        /// </summary>
        /// <param name="interpreterDirectoryPathProvider"></param>
        public RequirePathTransformer(IInterpreterDirectoryPathProvider interpreterDirectoryPathProvider)
        {
            _interpreterDirectoryPathProvider = interpreterDirectoryPathProvider;
        }
    }
}
