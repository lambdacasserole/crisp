﻿using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Packet.Interfaces.Configuration;
using Packet.Interfaces.Logging;
using Packet.Interfaces.Server;

namespace Packet.Server
{
    public class PacketHttpServer : IHttpServer
    {
        private TcpListener _listener;

        private readonly ILogger _logger;

        private readonly IPacketConfiguration _packetConfiguration;

        private readonly IHttpRequestParser _httpRequestParser;

        /// <summary>
        /// Gets whether or not this server is currently actively listening for requests.
        /// </summary>
        protected bool IsActive { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packetConfigurationProvider"></param>
        /// <param name="logger">The logger that should be used to log server events.</param>
        public PacketHttpServer(IPacketConfigurationProvider packetConfigurationProvider, ILogger logger, IHttpRequestParser httpRequestParser)
        {
            _packetConfiguration = packetConfigurationProvider.Get();
            _logger = logger;
            _httpRequestParser = httpRequestParser;
        }

        public void Listen()
        {
            // Start TCP listener.
            var ipAddress = IPAddress.Parse(_packetConfiguration.BindingIpAddress);
            _listener = new TcpListener(ipAddress, _packetConfiguration.Port);
            _listener.Start();
            IsActive = true;

            _logger.WriteLine($"Bound to IP {ipAddress} and listening on port {_packetConfiguration.Port}...");

            // While we're actively listening for connections.
            while (IsActive) // TODO: Weird way to listen for multiple requests.
            {
                _logger.WriteLine("Waiting for a request...");

                // Wait for TCP client connect.
                var client = _listener.AcceptTcpClient();

                _logger.WriteLine("Listener accepted TCP client...");

                using (var s = new StreamReader(client.GetStream()))
                {
                    var str = new UTF8Encoding().GetBytes(s.ReadToEnd());
                    var prs = _httpRequestParser.Parse(str);
                    _logger.Write(prs.Url);
                }

                // Pass request to processor and process in a new thread.
//                var processor = new HttpProcessor(client, this, Logger);
//                ThreadPool.QueueUserWorkItem(processor.Process);
            }
        }
    }
}
