using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using NestWrapper.Engine;
using Newtonsoft.Json.Linq;

namespace NestWrapper.Mock
{
    public class InMemoryConnection : IConnection, IDisposable
    {
        private static Dictionary<string, List<Tuple<string, string>>> _data = new Dictionary<string, List<Tuple<string, string>>>();
        private IEngineRequestSearch _mockEngine = new EngineRequestSearch();
        private readonly string _index;
        private readonly Dictionary<string, Type> _mappings = new Dictionary<string, Type>();

        public InMemoryConnection(string index)
        {
            _index = index;
        }

        public InMemoryConnection Map<T>(string type)
        {
            if (!_mappings.ContainsKey(type))
            {
                _mappings.Add(type, typeof(T));
            }
            return this;
        }

        public void UseSearchEngine(IEngineRequestSearch engine){
            _mockEngine = engine;
        }

        public void Dispose()
        {
            DisposeManagedResources();
        }

        public static void Clear()
        {
            _data.Clear();
        }

        protected virtual void DisposeManagedResources() { }

        TResponse IConnection.Request<TResponse>(RequestData requestData) => this.ReturnConnectionResponse<TResponse>(requestData);
        private TResponse ReturnConnectionResponse<TResponse>(RequestData requestData)
        {
            // TODO : To implement
            return default(TResponse);
        }

        Task<TResponse> IConnection.RequestAsync<TResponse>(RequestData requestData, CancellationToken cancellationToken) => this.ReturnConnectionStatusAsync<TResponse>(requestData, cancellationToken);

        private async Task<TResponse> ReturnConnectionStatusAsync<TResponse>(RequestData requestData, CancellationToken cancellationToken) where TResponse : class, IElasticsearchResponse, new()
        {
            MemoryStream stream = new MemoryStream();
            requestData.PostData.Write(stream, requestData.ConnectionSettings);
            string requestBody = Encoding.UTF8.GetString(stream.ToArray());

            var data = requestData.PostData;
            if (data != null)
            {
                using (var stream1 = requestData.MemoryStreamFactory.Create())
                {
                    if (requestData.HttpCompression)
                        using (var zipStream = new GZipStream(stream, CompressionMode.Compress))
                            await data.WriteAsync(zipStream, requestData.ConnectionSettings, cancellationToken).ConfigureAwait(false);
                    else
                        await data.WriteAsync(stream, requestData.ConnectionSettings, cancellationToken).ConfigureAwait(false);
                }
            }
            requestData.MadeItToResponse = true;

            if (requestData.PathAndQuery.Contains("_search"))
            {
                var result = _mockEngine.Request(requestData.PathAndQuery, _mappings, _index, _data, requestBody);
                Stream s = requestData.MemoryStreamFactory.Create(result);
                return await Elasticsearch.Net.ResponseBuilder.ToResponseAsync<TResponse>(requestData, null, 200, null, s, "application/json", cancellationToken);
            }
            else if (requestData.PathAndQuery.Contains("_update"))
            {
                string[] args = requestData.PathAndQuery.Split(new string[]{"/"}, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length >= 3)
                {
                    string index = args[0], type = args[1], id = args[2];

                    if (!_mappings.ContainsKey(type))
                        return await ReturnUpdateResponse<TResponse>(requestData, id, index, Result.Error, type);
                    
                    if (_data.ContainsKey(index))
                    {
                        List<Tuple<string, string>> values = _data[index];
                        Tuple<string, string> v = values.FirstOrDefault(t => t.Item1.Equals(id));
                        if (v != null)
                        {
                            JObject api = JObject.Parse(requestBody);
                            values.Remove(v);
                            values.Add(new Tuple<string, string>(id, api["doc"].ToString()));
                            return await ReturnUpdateResponse<TResponse>(requestData, id, index, Result.Updated, type);
                        }
                        else
                            return await ReturnUpdateResponse<TResponse>(requestData, id, index, Result.Error, type);
                    }
                    else
                        return await ReturnUpdateResponse<TResponse>(requestData, id, index, Result.Error, type);
                }
            }
            else
            {
                string[] args = requestData.PathAndQuery.Split(new string[]{"/"}, StringSplitOptions.RemoveEmptyEntries);
                if (args.Length >= 3)
                {
                    string index = args[0], type = args[1], idDoc = args[2];

                    // Type not mapped
                    if (!_mappings.ContainsKey(type))
                        return await ReturnIndexResponse<TResponse>(requestData, idDoc, index, Result.Error, type);

                    if (_data.ContainsKey(index))
                    {
                        List<Tuple<string, string>> values = _data[index];
                        Tuple<string, string> v = values.FirstOrDefault(t => t.Item1.Equals(idDoc));
                        if (v != null)
                            values.Remove(v);
                        values.Add(new Tuple<string, string>(idDoc, requestBody));
                    }
                    else
                    {
                        _data.Add(index, new List<Tuple<string, string>>() { new Tuple<string, string>(idDoc, requestBody) });
                    }

                    return await ReturnIndexResponse<TResponse>(requestData, idDoc, index, Result.Created, type);
                }
            }
            return await new TaskFactory<TResponse>().StartNew(() =>
            {
                return default(TResponse);
            });
        }

        private async Task<TResponse> ReturnUpdateResponse<TResponse>(RequestData requestData, string id, string index, Result res, string type)
            where TResponse : class, IElasticsearchResponse, new()
        {
            Stream s = requestData.MemoryStreamFactory.Create(ResponseBuilder.CreateUpdateResponse(id, index, res, type));
            return await Elasticsearch.Net.ResponseBuilder.ToResponseAsync<TResponse>(requestData, null, 200, null, s, "application/json");
        }

        private async Task<TResponse> ReturnIndexResponse<TResponse>(RequestData requestData, string id, string index, Result res, string type)
            where TResponse : class, IElasticsearchResponse, new()
        {
            Stream s = requestData.MemoryStreamFactory.Create(ResponseBuilder.CreateIndexResponse(id, index, res, type));
            return await Elasticsearch.Net.ResponseBuilder.ToResponseAsync<TResponse>(requestData, null, 200, null, s, "application/json");
        }
    }
}