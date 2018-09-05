using System;
using System.Collections.Generic;

namespace NestWrapper.Engine
{
    public interface IEngineRequestSearch 
    {
         byte[] Request(string pathAndQuery,
            Dictionary<string, Type> mappings,
            string index,
            Dictionary<string, List<Tuple<string, string>>> data,
            string requestBody);
    }
}