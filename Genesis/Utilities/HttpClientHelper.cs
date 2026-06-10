using Genesis.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Utilities
{
    /// <summary>
    /// TODO:
    /// </summary>
    public class HttpClientHelper
    {
        private static readonly HttpClient _client;
        private static readonly HttpClientHandler _handler;

        private static ILogger _logger;

        public static ILogger Logger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new GenesisLogger();
                }

                return _logger;
            }
        }

        static HttpClientHelper()
        {
            _handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip };

            _client = new HttpClient(_handler);
            _client.Timeout = TimeSpan.FromMinutes(5);
        }

        public static string Post(string url, string parameter)
        {
            string result = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(parameter))
                {
                    url = url + "?" + parameter;
                }
                var content = new StringContent(parameter, Encoding.UTF8, "application/json");
                HttpResponseMessage response = _client.PostAsync(url, content).Result;
                result = response.Content.ReadAsStringAsync().Result;

                if (!url.EndsWith("o2oqueryorder"))
                {
                    Logger.Information($"接口：{url}，参数：{parameter}，返回：{result}");
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Logger.Error("Http Post请求失败！", ex);
                Logger.Information($"接口：{url}，参数：{parameter}，返回：{ex.Message}");
            }

            return result;
        }

        public static string Get(string url, string parameter = null)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(parameter))
                url = url + "?" + parameter;

            try
            {
                //if (url.StartsWith("https"))
                //{
                //    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                //}
                //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

                HttpResponseMessage response = _client.GetAsync(url).Result;
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                Logger.Information($"方法：Get，接口：{url}，参数：{parameter}，返回：{ex.Message}");
            }

            return result;
        }

        public static string Delete(string url)
        {
            string result = string.Empty;

            try
            {
                HttpResponseMessage response = _client.DeleteAsync(url).Result;
                if ((int)response.StatusCode < 300)
                {
                    result = "{\"result\":\"ok\"}";
                }
                else
                {
                    result = response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Logger.Error("Http Delete请求失败！", ex);
            }

            return result;
        }

        public static string Put(string url, string parameter)
        {
            string result = string.Empty;

            try
            {
                HttpResponseMessage response = _client.PutAsync(url, new StringContent(parameter, Encoding.UTF8, "application/json")).Result;
                result = response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception ex)
            {
                //LogHelper.Logger.Error("Http Put请求失败！", ex);
            }

            return result;
        }

        public static async Task<string> PostAsync(string url, string parameter)
        {
            var response = await _client.PostAsync(new Uri(url), new StringContent(parameter, Encoding.UTF8, "application/json"));
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetAsync(string url, string parameter = null)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(parameter))
                url = url + "?" + parameter;

            try
            {
                using (var response = await _client.GetAsync(url))
                {
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                //LogHelper.Logger.Error("Http GetAsync请求失败！", ex);
            }

            return string.Empty;
        }
    }
}