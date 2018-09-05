using Elasticsearch.Net;
using Nest;
using System;

namespace NestWrapper.Builder
{
    public class BuilderElasticClient : IBuilderElasticClient
    {
        public virtual IElasticClient BuildElasticClient(string node, string index, Action<IConnection> action = null)
        {
            var nodeUri = new Uri(node);
            var settings = new ConnectionSettings(nodeUri);
            settings = settings.DefaultIndex(index);
            var client = new ElasticClient(settings);
            action?.Invoke(client.ConnectionSettings.Connection);
            return client;
        }
    }
}