using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Helpers
{
	public class SignHelper
	{
		/**
         * 对请求参数集进行MD5签名
         * @param params 待签名的请求参数集
         * @param secretCode 签名密码
         * @return 返回null 或 32位16进制大写字符串
         */
		public static string SignWithMD5(Dictionary<string, string> parameters, String secretCode)
		{
			var clearText = ConvertToParameterString(parameters, secretCode, false);
			byte[] byteData = Encoding.UTF8.GetBytes(clearText);
			MD5 oMd5 = MD5.Create();
			byte[] HashData = oMd5.ComputeHash(byteData);
			StringBuilder oSb = new StringBuilder();
			for (int x = 0; x < HashData.Length; x++)
			{
				//hexadecimal string value
				oSb.Append(HashData[x].ToString("x2"));
			}

			return oSb.ToString().ToUpper();
		}

		public static string ConvertToParameterString(Dictionary<string, string> parameters, String secretCode, bool includeSign)
		{
			StringBuilder sb = new StringBuilder();
			var keyList = parameters.Keys.ToList();
			keyList.Sort();
			foreach (var name in keyList)
			{
				bool isSign = string.Compare(name, "sign", true) == 0;
				if (!isSign || (isSign && includeSign))
				{
					var value = parameters[name];
					if (value == null)
					{
						value = "";
					}
					sb.Append($"{name}={value}&");
				}
			}
			// Remove last &.
			sb.Remove(sb.Length - 1, 1);
			if (!string.IsNullOrEmpty(secretCode))
			{
				sb.Append(secretCode);
			}

			return sb.ToString();
		}
	}
}
