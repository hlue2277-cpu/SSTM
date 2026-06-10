using SSTMTerminal.Converters;
using Newtonsoft.Json;

namespace SSTMTerminal.Helpers
{
    public class WebJsonConvertHelper
    {
        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, new CustemDateTimeConverter());
        }
    }
}