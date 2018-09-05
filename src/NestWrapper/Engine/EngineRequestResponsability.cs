using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace NestWrapper.Engine
{
    public class EngineRequestResponsability
    {
        /// <summary>
        /// The action to take
        /// </summary>
        private Func<JToken, List<object>, List<object>> _actionToTake;

        /// <summary>
        /// Function to check if action is for U
        /// </summary>
        private Func<JToken, bool> _check;

        /// <summary>
        /// The next socket message responsability
        /// </summary>
        EngineRequestResponsability _next;

        public EngineRequestResponsability(Func<JToken, bool> check, Func<JToken, List<object>, List<object>> actionToTake)
        {
            _check = check;
            _actionToTake = actionToTake;
        }

        public EngineRequestResponsability SetNext(Func<JToken, bool> check, Func<JToken, List<object>, List<object>> actionToTake)
        {
            _next = new EngineRequestResponsability(check, actionToTake);
            return _next;
        }

        public List<object> Execute(JToken token, List<object> data)
        {
            if (CriteriaMatches(token))
            {
                return _actionToTake(token, data);
            }

            if (_next != null)
                return _next.Execute(token, data);

            return null;
        }

        private bool CriteriaMatches(JToken token)
        {
            return _check(token);
        }
    }
}