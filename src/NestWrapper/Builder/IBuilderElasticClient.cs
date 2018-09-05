using System;
using Elasticsearch.Net;
using Nest;

namespace NestWrapper.Builder
{
    public interface IBuilderElasticClient
    {
        IElasticClient BuildElasticClient(string node, string index, Action<IConnection> action = null);
    }
}
