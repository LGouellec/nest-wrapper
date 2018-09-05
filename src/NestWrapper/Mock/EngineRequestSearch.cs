using System;
using System.Collections.Generic;
using System.Linq;
using Nest;
using NestWrapper.Crosscutting;
using NestWrapper.Engine;
using NestWrapper.Mock;
using Newtonsoft.Json.Linq;

namespace NestWrapper.Mock
{
    public class EngineRequestSearch : IEngineRequestSearch
    {
        private const string emptyQuery = "{}";
        private EngineRequestResponsability _engine = null;

        public EngineRequestSearch()
        {
            InitResponsability();
        }

        protected virtual void InitResponsability(){}

        #region Request

        public byte[] Request(
            string pathAndQuery,
            Dictionary<string, Type> mappings,
            string index,
            Dictionary<string, List<Tuple<string, string>>> data,
            string requestBody)
        {
            string[] val = pathAndQuery.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            string type = val[1];

            if (!mappings.ContainsKey(type))
                return new byte[0];

            Type typeDoc = mappings[type];

            if (!data.ContainsKey(index))
                return new byte[0];

            List<Tuple<string, string>> _data = data[index];

            if (requestBody.Equals(emptyQuery))
                return ReturnRequestBody(_data.Select(t => t.Item2).Select(s => s.Deserialize(typeDoc)), typeDoc);
            else
            {
                JToken query = JObject.Parse(requestBody)["query"];
                List<object> result = _engine.Execute(query, _data.Select(t => t.Item2).Select(s => s.Deserialize(typeDoc)).ToList());
                return ReturnRequestBody(result, typeDoc);
            }
        }

        private byte[] ReturnRequestBody(IEnumerable<object> datas, Type type)
        {
            return ResponseBuilder.CreateSearchResponse(datas.ToList(), obj =>
            {
                object[] objs = type.GetCustomAttributes(typeof(ElasticsearchTypeAttribute), true);
                if (objs.Length > 0)
                {
                    ElasticsearchTypeAttribute attr = objs[0] as ElasticsearchTypeAttribute;
                    string id = type.GetProperty(attr.IdProperty).GetValue(obj).ToString();
                    return id;
                }
                return "";
            });
        }

        #endregion
    }
}