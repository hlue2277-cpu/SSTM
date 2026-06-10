using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using Genesis.Logging;

namespace Genesis.Utilities
{
    public class HttpHelper
    {
        public static ILogger Logger = new GenesisLogger();

        /// <summary>
        /// 通过Http Get进行请求，并获取响应结果
        /// </summary>
        /// <param name="sync_url">请求Url</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>从网络获得数据</returns>
        public static string TransferSync_Get(string sync_url, string parameter, string macnum, int timeOut = 0)
        {
            // 开始请求
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(sync_url);
            var token = CryptHelper.GetSHA1HashFromString(request.RequestUri.AbsolutePath + parameter);
            request.Headers.Add("token", token);                //服务端验证使用
            request.Headers.Add("X-Request-DeviceId", macnum);
            // 请求方式
            request.Method = "GET";

            if (timeOut > 0)
            {
                // 请求超时间
                request.Timeout = timeOut;
            }

            //request.ServicePoint.Expect100Continue = false;

            // 获取响应数据
            StringBuilder response_value = new StringBuilder();
            // 获取响应流
            //System.Net.HttpWebResponse s = request.GetResponse() as System.Net.HttpWebResponse;
            //if (s.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            try
            {
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        response_value.Append(sr.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, sync_url+ token);
            }
            //}
            return response_value.ToString();
        }

        /// </summary>
        /// 通过Http Post进行请求，并获取响应结果
        /// </summary>
        /// <param name="sync_url">请求Url</param>
        /// <param name="parameter">需要传输的参数</param>
        /// <remarks> Create By Billows 2012-05-06</remarks>
        public static string TransferSync_Post(string sync_url, string parameter, string macnum, string temp = "")
        {
            string response_value = null;
            System.Net.ServicePointManager.Expect100Continue = false;
            // 开始请求
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(sync_url);
            var token = CryptHelper.GetSHA1HashFromString(request.RequestUri.AbsolutePath);
            request.Headers.Add("token", token);
            request.Headers.Add("X-Request-DeviceId", macnum);
            request.ContentType = "application/json;charset=UTF8";
            // 请求方式
            request.Method = "POST";
            // 内容类型
            // request.ContentType = "application/x-www-form-urlencoded;charset=UTF8";

            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;

            // 将字符串转化为字节
            byte[] transfer_arr = System.Text.Encoding.UTF8.GetBytes(parameter);

            // 设置请求内容长度
            request.ContentLength = transfer_arr.Length;
            //System.Net.HttpWebResponse s = request.GetResponse() as System.Net.HttpWebResponse;
            //if (s.StatusCode == System.Net.HttpStatusCode.OK)
            //{
                //获得请求流
                using (Stream writer = request.GetRequestStream())
                {
                    // 将请求参数写入流
                    writer.Write(transfer_arr, 0, transfer_arr.Length);
                }

                // 获取响应流
                using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    using (System.IO.StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {
                        response_value = sr.ReadToEnd();
                    }
                }
            //}
            return response_value;
        }

        /// </summary>
        /// 通过Http Patch进行请求，并获取响应结果
        /// </summary>
        /// <param name="sync_url">请求Url</param>
        /// <param name="parameter">需要传输的参数</param>
        /// <remarks> Create By Billows 2012-05-06</remarks>
        public static string TransferSync_Patch(string sync_url, string parameter, string macnum, string temp = "")
        {
            string response_value = null;
            System.Net.ServicePointManager.Expect100Continue = false;
            // 开始请求
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(sync_url);
            var token = CryptHelper.GetSHA1HashFromString(request.RequestUri.AbsolutePath);
            request.Headers.Add("token", token);
            request.Headers.Add("X-Request-DeviceId", macnum);
            // 请求方式
            request.Method = "PATCH";
            // 内容类型
            request.ContentType = "application/json;charset=UTF8";

            request.KeepAlive = false;
            request.ProtocolVersion = HttpVersion.Version10;

            // 将字符串转化为字节
            byte[] transfer_arr = System.Text.Encoding.UTF8.GetBytes(parameter);

            // 设置请求内容长度
            request.ContentLength = transfer_arr.Length;

            //获得请求流
            using (Stream writer = request.GetRequestStream())
            {
                // 将请求参数写入流
                writer.Write(transfer_arr, 0, transfer_arr.Length);
            }

            // 获取响应流
            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                using (System.IO.StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    response_value = sr.ReadToEnd();
                }
            }

            return response_value;
        }

        /// <summary>
        /// 将本地文件上传到指定的服务器(HttpWebRequest方法)
        /// </summary>
        /// <param name="sync_url">请求Url</param>
        /// <param name="parameter">需要传输的参数</param>
        /// <param name="fileNamePath">要上传的本地文件（全路径）</param>
        /// <param name="progressBar">上传进度条（进度，已上传的字节数）</param>
        /// <returns>响应信息</returns>
        public static string UploadSync_Post(string sync_url, string parameter, string fileNamePath, Action<int, long> progress)
        {
            string response_value = string.Empty;

            //时间戳
            string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");

            //请求头部信息
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(strBoundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("file");
            sb.Append("\"; filename=\"");
            sb.Append(Path.GetFileName(fileNamePath));
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");

            // 开始请求
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(new Uri(sync_url + "?" + parameter));
            // 请求方式
            request.Method = "POST";
            //对发送的数据不使用缓存
            request.AllowWriteStreamBuffering = false;
            //设置获得响应的超时时间（300秒）
            request.Timeout = 300000;
            // 内容类型
            request.ContentType = "multipart/form-data; boundary=" + strBoundary;

            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "--\r\n");

            // 要上传的文件
            using (FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read))
            {
                long fileLength = fs.Length;
                using (BinaryReader br = new BinaryReader(fs))
                {
                    long length = fileLength + postHeaderBytes.Length + boundaryBytes.Length;
                    request.ContentLength = length;

                    //每次上传4k
                    int bufferLength = 4096;
                    byte[] buffer = new byte[bufferLength];
                    int size = br.Read(buffer, 0, bufferLength);

                    //已上传的字节数
                    long offset = 0;

                    //获得请求流
                    using (Stream writer = request.GetRequestStream())
                    {
                        //发送请求头部消息
                        writer.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                        //发送内容
                        while (size > 0)
                        {
                            //每次请求发送4k内容
                            writer.Write(buffer, 0, size);

                            offset += size;
                            //回调上传进度条
                            progress((int)(offset / fileLength / 100), offset);
                            size = br.Read(buffer, 0, bufferLength);
                        }

                        //添加尾部的时间戳
                        writer.Write(boundaryBytes, 0, boundaryBytes.Length);
                    }
                }
            }

            // 获取响应流
            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                using (System.IO.StreamReader sr = new StreamReader(response.GetResponseStream()))
                {
                    response_value = sr.ReadToEnd();
                }
            }

            return response_value;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <param name="sync_url">请求Url</param>
        /// <param name="parameter">验证码ID</param>
        /// <param name="timeOut">超时时间</param>
        /// <returns>验证码位图</returns>
        public static Bitmap CaptchaSync_Post(string sync_url, ref string parameter, int timeOut = 0)
        {
            Bitmap sourcebm = null;
            CookieContainer cookie = new CookieContainer();
            bool isRefresh = !string.IsNullOrEmpty(parameter);
            string requestUriString = isRefresh ? (string.Format("{0}?captchaId={1}", sync_url, parameter)) : sync_url;

            // 开始请求
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)WebRequest.Create(requestUriString);

            //关联cookie
            request.CookieContainer = cookie;

            // 请求方式
            request.Method = "POST";

            if (timeOut > 0)
            {
                // 请求超时间
                request.Timeout = timeOut;
            }

            // 获取响应流
            using (System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)request.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    sourcebm = new Bitmap(responseStream);//初始化Bitmap图片
                }

                if (sourcebm != null && !isRefresh)
                {
                    var responseCookie = response.Cookies["captchaId"];
                    parameter = responseCookie.Value;
                }
            }

            return sourcebm;
        }
    }
}