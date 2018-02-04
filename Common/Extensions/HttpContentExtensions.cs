using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JSONCapital.Common.Extensions
{
    public static class HttpContentExtensions
    {
        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            T value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content, params JsonConverter[] converters)
        {
            string json = await content.ReadAsStringAsync();
            T value = JsonConvert.DeserializeObject<T>(json, converters);
            return value;
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content, JsonSerializerSettings settings)
        {
            string json = await content.ReadAsStringAsync();
            T value = JsonConvert.DeserializeObject<T>(json, settings);
            return value;
        }
    }
}
