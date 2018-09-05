using System;
using Newtonsoft.Json;

namespace NestWrapper.Crosscutting
{
    public static class JsonTools
    {
        public static T Deserialize<T>(this string content)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(content);
            }
            catch (Exception e)
            {
                // TODO : log exception
                return default(T);
            }
        }

        public static object Deserialize(this string content, Type type)
        {
            try
            {
                return JsonConvert.DeserializeObject(content, type);
            }
            catch (Exception e)
            {
                // TODO : log exception
                return default(object);
            }
        }

        public static string Serialize<T>(this T content)
        {
            try
            {
                return JsonConvert.SerializeObject(content);
            }
            catch (Exception e)
            {
                // TODO : log exception
                return default(string);
            }
        }
    }
}