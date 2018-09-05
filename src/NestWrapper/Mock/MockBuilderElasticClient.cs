using System;
using System.Linq;
using System.Text;
using Elasticsearch.Net;
using Nest;
using NestWrapper.Builder;
using Newtonsoft.Json;

namespace NestWrapper.Mock
{
    public class MockBuilderElasticClient : IBuilderElasticClient
    {
        public IElasticClient BuildElasticClient(string node, string index, Action<IConnection> action = null)
        {
            var connection = new NestWrapper.Mock.InMemoryConnection(index);
            var connectionPool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
            var settings = new ConnectionSettings(connectionPool, connection);
            settings = settings.DefaultIndex(index);
            var client = new ElasticClient(settings);
            action?.Invoke(connection);
            return client;
        }
    }
}