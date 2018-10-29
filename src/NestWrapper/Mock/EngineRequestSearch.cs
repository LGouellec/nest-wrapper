using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Nest;
using NestWrapper.Crosscutting;
using NestWrapper.Engine;
using NestWrapper.Mock;
using Newtonsoft.Json.Linq;

namespace NestWrapper.Mock
{
    public class EngineRequestSearch : IEngineRequestSearch
    {
        protected const string emptyQuery = "{}";
        protected EngineRequestResponsability _engine = null;

        public EngineRequestSearch()
        {
            InitResponsability();
        }

        protected virtual void InitResponsability(){}

        #region Request

        public virtual byte[] Request(
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
            List<Tuple<string, string>> _data = new List<Tuple<string, string>>();

            if (!data.ContainsKey(index))
            {
                List<string> indexes = new List<string>();
                string pattern = index;
                Regex regex = new Regex(pattern);
                foreach(string ind in data.Select(kp => kp.Key))
                    if(regex.IsMatch(ind))
                        indexes.Add(ind);

                if (indexes.Count == 0)
                    return new byte[0];
                else
                {
                    foreach(string ind in indexes)
                        _data.AddRange(data[ind]);
                }
            }
            else
                _data = data[index];

            JToken query = JObject.Parse(requestBody)["query"];
            if (query == null)
                return ReturnRequestBody(_data.Select(t => t.Item2).Select(s => s.Deserialize(typeDoc)), typeDoc);
            else
            {
                List<object> result = _engine.Execute(query, _data.Select(t => t.Item2).Select(s => s.Deserialize(typeDoc)).ToList());
                return ReturnRequestBody(result, typeDoc);
            }
        }

        protected virtual byte[] ReturnRequestBody(IEnumerable<object> datas, Type type)
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