﻿using System.Collections.Generic;

using Packet.Interfaces.Configuration;

namespace Packet.Configuration
{
    public class PacketConfiguration : IPacketConfiguration
    {
        public IEnumerable<string> CrispFileExtensions { get; set; }
        
        public IDictionary<string, string> MimeTypeMappings { get; set; }

        public string BadRequestErrorPage { get; set; }
        
        public string InternalServerErrorPage { get; set; }
        
        public string NotFoundErrorPage { get; set; }

        public string ForbiddenErrorPage { get; set; }
        
        public IEnumerable<string> DirectoryIndices { get; set; } 
        
        public IEnumerable<string> DoNotServePatterns { get; set; }
        
        public string BindingIpAddress { get; set; }

        public int Port { get; set; }

        public int MaxPostSize { get; set; }

        public string WebRoot { get; set; }
    }
}
