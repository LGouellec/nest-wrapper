using System;
using Nest;

namespace dTris_Inventory_Api.Builder
{
    public class BuilderElasticClient : IBuilderElasticClient
    {
        public virtual IElasticClient BuildElasticClient(string node, string index)
        {
            var node = new Uri(configuration.Node);
            var settings = new ConnectionSettings(node);
            settings = settings.DefaultIndex(index);
            var client = new ElasticClient(settings);
            return client;
        }
    }
}