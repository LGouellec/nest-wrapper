using System;
using Elasticsearch.Net;
using Nest;
using NestWrapper.Engine;

namespace NestWrapper
{
    public static class NestWrapperExtensions
    {
        static void Map<T>(this IConnection connection, string type)
        {
            if (connection is NestWrapper.Mock.InMemoryConnection)
            {
                var wrapperConnection = connection as NestWrapper.Mock.InMemoryConnection;
                wrapperConnection.Map<T>(type);
            }
        }

        static void UseSearchEngine(this IConnection connection, IEngineRequestSearch engine)
        {
            if (connection is NestWrapper.Mock.InMemoryConnection)
            {
                var wrapperConnection = connection as NestWrapper.Mock.InMemoryConnection;
                wrapperConnection.UseSearchEngine(engine);
            }
        }
    }
}
