using System;
using System.Web;
using Newtonsoft.Json;

namespace Hexon.MvcTrig
{
    internal class HttpHeaderStringEncodeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(HttpUtility.UrlPathEncode((string)value));
        }
    }
}