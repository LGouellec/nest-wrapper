using System;
using System.Linq;
using System.Text;
using dTris_Inventory_Api.Builder;
using dTris_Inventory_Api.Crosscutting;
using dTris_Inventory_Api.Models;
using Elasticsearch.Net;
using Nest;
using NestWrapper.Builder;
using Newtonsoft.Json;

namespace dTris_Inventory_Api.Mock.Nest
{
    public class MockBuilderElasticClient : IBuilderElasticClient
    {
        public IElasticClient BuildElasticClient(string node, string index)
        {
            var connection = new Mock.Nest.InMemoryConnection(index);
            connection.Map<Asset>("asset").Map<Discovery>("discovery");

            var connectionPool = new SingleNodeConnectionPool(new Uri("http://localhost:9200"));
            var settings = new ConnectionSettings(connectionPool, connection);
            settings = settings.DefaultIndex(index);
            var client = new ElasticClient(settings);
            return client;
        }
    }
}