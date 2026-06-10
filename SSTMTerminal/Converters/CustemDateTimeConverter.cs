using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace SSTMTerminal.Converters
{
    public class CustemDateTimeConverter : DateTimeConverterBase
    {
        private static IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return dtConverter.ReadJson(reader, objectType, existingValue, serializer);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            DateTime result;
            if (value == null
                || (value != null
                    && DateTime.TryParse(value.ToString(), out result)
                    && (result == DateTime.MinValue || result == DateTime.Parse("0001-01-01 08:00:00"))))
            {
                writer.WriteValue("-");
            }
            else
            {
                dtConverter.WriteJson(writer, value, serializer);
            }
        }
    }
}