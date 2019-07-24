using System;
using KenticoInspector.Core.Models;
using Newtonsoft.Json;

namespace KenticoInspector.Core
{
    /// <summary>
    /// Implementation of <see cref="JsonConverter"/> that allows for the serialization of <see cref="Term"/>s.
    /// </summary>
    public class TermConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            switch (objectType)
            {
                case var _ when objectType == typeof(string):
                case var _ when objectType == typeof(Term):
                    return true;

                default:
                    return false;
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            string stringTerm = (Term)value;

            serializer.Serialize(writer, stringTerm);
        }
    }
}