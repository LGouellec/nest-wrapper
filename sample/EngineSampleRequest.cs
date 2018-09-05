using System;
using System.Collections.Generic;
using System.Linq;
using NestWrapper.Engine;
using NestWrapper.Mock;
using Newtonsoft.Json.Linq;
using sample.Models;

namespace sample
{
    public class EngineSampleRequest : EngineRequestSearch
    {
        protected override void InitResponsability()
        {
            _engine = new EngineRequestResponsability(CanPersonneByName, PersonneByName);
        }

        private bool CanPersonneByName(JToken arg)
        {
            return arg["term"] != null && arg["term"]["name"] != null;
        }

        private List<object> PersonneByName(JToken arg1, List<object> arg2)
        {
            return arg2.Where(o =>
            {
                string name = arg1["term"]["name"]["value"].ToString();
                return o is Personne && ((Personne)o).Name.Equals(name);
            }).ToList();
        }
    }
}