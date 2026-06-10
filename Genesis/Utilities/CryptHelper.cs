using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Genesis.Logging;

namespace Genesis.Utilities
{
    /// <summary>
    /// 加密/解密帮助类
    /// </summary>
    public class CryptHelper
    {
        public static ILogger Logger = new GenesisLogger();

        #region MD5

        /// <summary>
        /// 将指定的字符串进行MD5加密码
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <remarks>Create By Billows 2012/04/26</remarks>
        /// <returns>加密后的字符串</returns>
        public static string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        #endregion

        #region DES3 ECB 字符串加密

        public static readonly string Des3key = "WF2IZXPu2s9eJ18qp7+WhK2CYHJMHQDt";

        /// <summary>
        /// 获得Des3加密后的字符串
        /// </summary>
        /// <returns></returns>
        public static string Des3Encode(string input)
        {
            string rtn = "";
            try
            {
                System.Text.Encoding utf8 = System.Text.Encoding.UTF8;
                byte[] key = Convert.FromBase64String(Des3key);
                byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                byte[] data = utf8.GetBytes(input);
                byte[] str1 = Des3EncodeECB(key, iv, data);
                rtn = Convert.ToBase64String(str1);
            }
            catch
            {
                Logger.Error(input + "加密失败");
            }
            return rtn;
        }

        /// <summary>
        /// 从服务器加密的字符串获得第三方票务系统的密码
        /// </summary>
        /// <param name="printNo"></param>
        /// <returns></returns>
        public static string Des3Decode(string printNo)
        {
            string rtn = "";
            try
            {
                byte[] str1 = Convert.FromBase64String(printNo);
                byte[] key = Convert.FromBase64String(Des3key);
                byte[] iv = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                byte[] str2 = Des3DecodeECB(key, iv, str1);
                rtn = System.Text.Encoding.UTF8.GetString(str2);
                rtn = TrimSpace(rtn);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return rtn;
        }

        /// <summary>
        /// DES3 ECB模式加密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">明文的byte数组</param>
        /// <returns>密文的byte数组</returns>
        private static byte[] Des3EncodeECB(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream mStream = new MemoryStream();
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;
                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(mStream,
                    tdsp.CreateEncryptor(key, iv),
                    CryptoStreamMode.Write);
                // Write the byte array to the crypto stream and flush it.
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                // Get an array of bytes from the 
                // MemoryStream that holds the 
                // encrypted data.
                byte[] ret = mStream.ToArray();
                // Close the streams.
                cStream.Close();
                mStream.Close();
                // Return the encrypted buffer.
                return ret;
            }
            catch (CryptographicException e)
            {
                Logger.Error(e);
            }

            return null;
        }

        /// <summary>
        /// DES3 ECB模式解密
        /// </summary>
        /// <param name="key">密钥</param>
        /// <param name="iv">IV(当模式为ECB时，IV无用)</param>
        /// <param name="str">密文的byte数组</param>
        /// <returns>明文的byte数组</returns>
        private static byte[] Des3DecodeECB(byte[] key, byte[] iv, byte[] data)
        {
            try
            {
                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                MemoryStream msDecrypt = new MemoryStream(data);
                TripleDESCryptoServiceProvider tdsp = new TripleDESCryptoServiceProvider();
                tdsp.Mode = CipherMode.ECB;
                tdsp.Padding = PaddingMode.PKCS7;
                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                    tdsp.CreateDecryptor(key, iv),
                    CryptoStreamMode.Read);
                // Create buffer to hold the decrypted data.
                byte[] fromEncrypt = new byte[data.Length];
                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);
                //Convert the buffer into a string and return it.
                return fromEncrypt;
            }
            catch (CryptographicException e)
            {
                Logger.Error(e);
            }

            return null;
        }

        private static string TrimSpace(string msg)
        {
            return msg.Replace("\0", "");
        }

        #endregion

        #region DES

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Key_64"></param>
        /// <param name="Iv_64"></param>
        /// <returns></returns>
        public static string DESEncode(string data, string Key_64, string Iv_64)
        {
            string KEY_64 = Key_64;
            string IV_64 = Iv_64;
            try
            {
                byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);

                byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                int i = cryptoProvider.KeySize;
                MemoryStream ms = new MemoryStream();
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);
                StreamWriter sw = new StreamWriter(cst);
                sw.Write(data);
                sw.Flush();
                cst.FlushFinalBlock();
                sw.Flush();
                return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="Key_64"></param>
        /// <param name="Iv_64"></param>
        /// <returns></returns>
        public static string DESDecode(string data, string Key_64, string Iv_64)
        {
            string KEY_64 = Key_64;
            string IV_64 = Iv_64;
            try
            {

                byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(KEY_64);

                byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(IV_64);

                byte[] byEnc;
                byEnc = Convert.FromBase64String(data); //把需要解密的字符串转为8位无符号数组
                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream ms = new MemoryStream(byEnc);
                CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
                StreamReader sr = new StreamReader(cst);
                return sr.ReadToEnd();
            }
            catch (Exception x)
            {
                return x.Message;
            }
        }

        #endregion

        #region API加密

        /// <summary>
        /// API参数加密
        /// </summary>
        /// <param name="sbSyncOrderUrl"></param>
        /// <param name="list"></param>
        /// <param name="strPar"></param>
        /// <returns></returns>
        public static void APIParameterEncrypt(StringBuilder sbSyncOrderUrl, string MovieKey, string MoviePrivateKey, List<string> list)
        {
            string strPar = "appkey=" + MovieKey;
            sbSyncOrderUrl.Append(strPar + "&");
            list.Add(strPar);
            strPar = "timestamp=" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sbSyncOrderUrl.Append(strPar + "&");
            list.Add(strPar);
            strPar = "v=1.0";
            sbSyncOrderUrl.Append(strPar + "&");
            list.Add(strPar);
            strPar = "format=xml";
            sbSyncOrderUrl.Append(strPar + "&");
            list.Add(strPar);
            sbSyncOrderUrl.Append("sign=" + GetSign(list, MoviePrivateKey));
            sbSyncOrderUrl.Append("&signmethod=MD5");
        }

        /// <summary>
        /// 通过参数列和私密钥获得签名
        /// </summary>
        /// <param name="parmList">参数列</param>
        /// <param name="strPrivateKey">私密钥</param>
        /// <returns>签名</returns>
        /// <remarks>
        /// Create By Scenery 2012-10-26
        /// </remarks>
        private static string GetSign(List<string> parmList, string strPrivateKey)
        {
            StringBuilder sbSign = new StringBuilder();
            if (parmList != null && parmList.Count > 0)
            {
                parmList.Sort(new CompareString());
                foreach (string item in parmList)
                {
                    sbSign.Append(item + "&");
                }
                return GetMd5Hash(sbSign.ToString().TrimEnd('&') + strPrivateKey);
            }

            return string.Empty;
        }

        #endregion

        #region SHA-1 加密
        public static readonly string SHAkey = "302782f3340645f0a97814731ac6d5ae";
        public static string GetSHA1HashFromString(string strData)
        {
            byte[] bytValue = System.Text.Encoding.UTF8.GetBytes(strData+ SHAkey);
            try
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                //SHA512 sha512 = new SHA512CryptoServiceProvider();
                byte[] retVal = sha1.ComputeHash(bytValue);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("GetSHA512HashFromString() fail,error:" + ex.Message);
            }
        }

        #endregion
    }

    /// <summary>
    /// 字符串排序
    /// </summary>
    public class CompareString : IComparer<string>
    {
        #region IComparer<string> 成员
        /// <summary>
        /// 字符串排序
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(string x, string y)
        {
            return String.CompareOrdinal(x, y);
        }
        #endregion
    }
}
