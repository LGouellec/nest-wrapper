using System;
using Elasticsearch.Net;
using Nest;
using NestWrapper.Engine;

namespace NestWrapper
{
    public static class NestWrapperExtensions
    {
        public static IConnection Map<T>(this IConnection connection, string type)
        {
            if (connection is NestWrapper.Mock.InMemoryConnection)
            {
                var wrapperConnection = connection as NestWrapper.Mock.InMemoryConnection;
                wrapperConnection.Map<T>(type);
            }
            return connection;
        }

        public static IConnection UseSearchEngine(this IConnection connection, IEngineRequestSearch engine)
        {
            if (connection is NestWrapper.Mock.InMemoryConnection)
            {
                var wrapperConnection = connection as NestWrapper.Mock.InMemoryConnection;
                wrapperConnection.UseSearchEngine(engine);
            }
            return connection;
        }
    }
}
