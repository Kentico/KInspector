using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace KenticoInspector.Core.Converters
{
    public class VersionListConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<Version>));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Converters.Add(new VersionObjectConverter());

            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            serializer.Converters.Add(new VersionObjectConverter());

            return serializer.Deserialize(reader, typeof(Version));
        }
    }
}