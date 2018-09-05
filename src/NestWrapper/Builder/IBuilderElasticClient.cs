using System;

namespace NestWrapper.Builder
{
    public interface IBuilderElasticClient
    {
         IElasticClient BuildElasticClient(string node, string index);
    }
}
