using System;
using System.Collections.Generic;
using System.Linq;
using JSONCapital.Data.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSONCapital.Services.Json.Converters
{
    /// <summary>
    /// Handles converting JSON string values into a C# boolean data type.
    /// </summary>
    public class TradeConverter : JsonConverter
    {
        #region Overrides of JsonConverter

        /// <summary>
        /// Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type objectType)
        {
            // Handle only boolean types.
            return objectType == typeof(IEnumerable<Trade>);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>
        /// The object value.
        /// </returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken t = JToken.FromObject(reader.Value);
			
            var lstTrades = new List<Trade>();

            if (t.Type != JTokenType.Object)
            {
                return lstTrades;
            }
            else
            {

                JObject o = (JObject)t;

                foreach (JObject content in o.Children<JObject>())
                {
                    foreach (JProperty prop in content.Properties())
                    {
                        // https://stackoverflow.com/questions/21002297/getting-the-name-key-of-a-jtoken-with-json-net
                        Console.WriteLine(prop.Name);
                        var trade = prop.Value.ToObject<Trade>(); // deserialize objec in to trade
                        trade.TradeID = int.Parse(prop.Name); // trade id is the json object property name
                        lstTrades.Add(trade);
                    }
                }

            }

			return lstTrades;
        }

        /// <summary>
        /// Specifies that this converter will not participate in writing results.
        /// </summary>
        public override bool CanWrite { get { return false; } }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter"/> to write to.</param><param name="value">The value.</param><param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        #endregion Overrides of JsonConverter
    }
}
