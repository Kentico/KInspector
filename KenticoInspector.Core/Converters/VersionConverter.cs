using Newtonsoft.Json;

using System;

namespace KenticoInspector.Core.Converters
{
    public class VersionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var version = value as Version;
            serializer.Serialize(writer, new
            {
                version.Major,
                version.MajorRevision,
                version.Minor,
                version.MinorRevision
            });
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<Version>(reader);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Version);
        }
    }
}
