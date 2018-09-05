using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Nest;
using NestWrapper.Builder;
using NestWrapper;
using sample.Models;
using System;

namespace sample
{
    public class ElasticService : IElasticService, IHostedService
    {
        private readonly IBuilderElasticClient _builder;
        private readonly IElasticClient _client;

        public ElasticService(IBuilderElasticClient builder)
        {
            _builder = builder;

            _client = _builder.BuildElasticClient("http://localhost:9200", "personne_index", (connection) => {
                connection.Map<Personne>("personne").UseSearchEngine(new EngineSampleRequest());
            });

            // add few personne in mock elastic
            _client.IndexDocumentAsync(new Personne(){ID = 1, Name = "Toto", LastName = "Toto Last Name"});
            _client.IndexDocumentAsync(new Personne(){ID = 2, Name = "Titi", LastName = "Titi Last Name"});
            _client.IndexDocumentAsync(new Personne(){ID = 3, Name = "Tata", LastName = "Tata Last Name"});
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return new TaskFactory().StartNew(() =>
            {
                ISearchResponse<Personne> resp = _client.SearchAsync<Personne>(s => s).GetAwaiter().GetResult(); 
                foreach(Personne p in resp.Documents)
                    Console.WriteLine(p);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {            
            return new TaskFactory().StartNew(() => {
                // Clear elasticsearch mock
                NestWrapper.Mock.InMemoryConnection.Clear();
            });
        }
    }
}