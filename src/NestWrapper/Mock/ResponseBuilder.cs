using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nest;
using NestWrapper.Crosscutting;

namespace NestWrapper.Mock
{
    public class ResponseBuilder
    {
         public static byte[] CreateIndexResponse(string id, string index, Result result, string type)
        {
            var response = new
            {
                Id = id,
                Index = index,
                Type = type,
                Result = result
            };
            return Encoding.UTF8.GetBytes(response.Serialize());
        }

        internal static byte[] CreateUpdateResponse(string id, string index, Result result, string type)
        {
            var response = new
            {
                Id = id,
                Index = index,
                Type = type,
                Result = result,
                IsValid = true,
                ShardsHit = new {
                    Failed = 0,
                    Successful = 1,
                    Total = 2
                }
            };
            return Encoding.UTF8.GetBytes(response.Serialize());
        }

        public static byte[] CreateSearchResponse(List<object> objects, Func<object, string> getId)
        {
            var response = new
            {
                took = 1,
                timed_out = false,
                _shards = new
                {
                    total = 2,
                    successful = 2,
                    failed = 0
                },
                hits = new
                {
                    total = 25,
                    max_score = 1.0,
                    hits = objects.Select(i => (object)new
                    {
                        _index = i.GetType().Name,
                        _type = i.GetType().Name,
                        _id = getId(i),
                        _score = 1.0,
                        _source = i
                    }).ToArray()
                },
                documents = objects
            };
            return Encoding.UTF8.GetBytes(response.Serialize());
        }
    }
}