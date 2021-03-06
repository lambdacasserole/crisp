﻿using System.Text;
using Crisp.Interfaces.Shared;
using Crisp.Shared;

using Packet.Configuration;
using Packet.Handlers;
using Packet.Interfaces.Configuration;
using Packet.Interfaces.Logging;
using Packet.Interfaces.Server;
using Packet.Logging;
using Packet.Server;

using SimpleInjector;

namespace Packet.IoC
{
    public static class HttpServerFactory
    {
        public static IHttpServer GetPacketHttpServer()
        {
            // Dependency injection.
            var container = new Container();
            container.Register<IEncodingProvider>(() => new EncodingProvider(new UTF8Encoding()));
            container.Register<IErrorPageContentRetriever, ErrorPageContentRetriever>();
            container.Register<IInterpreterFilePathProvider, InterpreterFilePathProvider>();
            container.Register<IInterpreterDirectoryPathProvider, InterpreterDirectoryPathProvider>();
            container.Register<IPacketConfigurationFileNameProvider>(() => new PacketConfigurationFileNameProvider("packet.json"));
            container.Register<IRawPacketConfigurationProvider, RawPacketConfigurationProvider>();
            container.Register<IPacketConfigurationProvider, PacketConfigurationProvider>();
            container.RegisterCollection<IHttpRequestParser>(new[]
            {
                typeof (FullHttpRequestParser),
                typeof (SimpleHttpRequestParser)
            });
            container.Register<IHttpRequestParser, ChainingHttpRequestParser>();
            container.Register<ILogger, ConsoleWindowLogger>();
            container.Register<IHttpRequestReader, HttpRequestReader>();
            container.Register<IHttpServer, PacketHttpServer>();
            container.Register<IUrlResolver, UrlResolver>();
            container.RegisterCollection<IHttpRequestHandler>(new[]
            {
                typeof (ForbiddenHttpRequestHandler),
                typeof (NotFoundHttpRequestHandler),
                typeof (DynamicPageHttpRequestHandler),
                typeof (StaticFileHttpRequestHandler)
            });
            container.Register<IHttpRequestHandler, ChainingHttpRequestHandler>();
            container.Register<IHttpConnectionHandler, ThreadedHttpConnectionHandler>();
            container.Register<IHttpRequestConverter, HttpRequestConverter>();
            container.Verify();

            return container.GetInstance<IHttpServer>();
        }
    }
}
