using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace Genesis.Utilities
{
    /// <summary>
    /// Json操作帮助类
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 将对象转换成Json
        /// </summary>
        /// <param name="_object">对象</param>
        /// <returns>Json</returns>
        /// <remarks>
        /// Create By Scenery 2012-12-5
        /// </remarks>
        public static string ObjectToJson<T>(T _object)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(_object.GetType());
            string json = null;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, _object);
                json = Encoding.UTF8.GetString(stream.ToArray());
                json = json.Replace("\0", "");
            }
            return json;
        }

        /// <summary>
        /// 将JSon转换成对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">Json</param>
        /// <returns>对象</returns>
        /// <remarks>
        /// Create By Scenery 2012-12-5
        /// </remarks>
        public static T JsonToObject<T>(string json)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            T obj = default(T);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                obj = (T)serializer.ReadObject(ms);
            }
            return obj;
        }

        /// <summary>
        /// JSON序列化
        /// </summary>
        public static string JsonSerializer<T>(T t)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, t);
            string jsonString = Encoding.UTF8.GetString(ms.ToArray());
            ms.Close();
            //替换Json的Date字符串
            string p = @"\\/Date\((\d+)\+\d+\)\\/";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            return jsonString;
        }

        public static string ConvertToDateTime(string jsonString)
        {
            if(jsonString == null)
            {
                return jsonString;
            }

            var startIndex = jsonString.IndexOf("(");
            var endIndex = jsonString.IndexOf("+");
            if(startIndex != -1 && endIndex != -1 && startIndex < endIndex && jsonString.Contains(@"/Date(") && jsonString.Contains(@")"))
            {
                var msValue = jsonString.Substring(startIndex + 1, endIndex - startIndex - 1);

                DateTime dt = new DateTime(1970, 1, 1);
                dt = dt.AddMilliseconds(long.Parse(msValue));
                dt = dt.ToLocalTime();
                return dt.ToString("yyyy-MM-dd HH:mm:ss");
            }

            return jsonString;
        }

        /// <summary>
        /// JSON反序列化
        /// </summary>
        public static T JsonDeserialize<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(ms);
            return obj;
        }

        /// <summary>
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>
        /// 将时间字符串转为Json时间
        /// </summary>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
    }
}
