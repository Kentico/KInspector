using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

namespace KenticoInspector.Core.Converters
{
    public class VersionObjectConverter : JsonConverter<Version>
    {
        public override void WriteJson(JsonWriter writer, Version value, JsonSerializer serializer)
        {
            JObject jsonVersion = new JObject
        {
            { "major", value.Major },
            { "minor", value.Minor },
            { "build", value.Build },
        };
            jsonVersion.WriteTo(writer);
        }

        public override Version ReadJson(JsonReader reader, Type objectType, Version existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jsonVersion = JObject.Load(reader);
            return new Version((int)jsonVersion["major"], (int)jsonVersion["minor"], (int)jsonVersion["build"]);
        }
    }
}