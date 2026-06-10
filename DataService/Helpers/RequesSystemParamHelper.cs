using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService.Helpers
{
	public class RequesSystemParamHelper
	{
		private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static Dictionary<string, string> GenerateSystemParameter()
		{
			var parameters = new Dictionary<string, string>();
			parameters.Add("timestamp", "" + (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds);
			parameters.Add("v", "1.0");
			parameters.Add("appkey", ConfigurationManager.AppSettings["AppKey"]);

			return parameters;
		}
	}
}
