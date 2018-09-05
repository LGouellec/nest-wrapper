using System;
using System.Collections.Generic;
using NestWrapper.Engine;
using NestWrapper.Mock;

namespace sample
{
    public class EngineSampleRequest : EngineRequestSearch
    {
        protected override void InitResponsability()
        {
            _engine = new EngineRequestResponsability((token) => true, (token, objs) => objs);
        }
    }
}