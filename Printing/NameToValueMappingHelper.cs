using DataService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Printing
{
	public static class NameToValueMappingHelper
	{
		private static ScriptOptions expressionOptions;
		private static PrintingNameToValueMappingConfig config;
		// The key is ticket's scheduleId + expression
		private static Dictionary<string, Func<TicketResponseData, DateTime, string>> expressionDic;

		static NameToValueMappingHelper()
		{
			expressionOptions = ScriptOptions.Default.AddReferences(typeof(TicketResponseData).Assembly);
			var configFileName = "PrintingNameToValueMappingConfig.xml";
			var cofigFilePath = string.Format("{0}\\{1}", AppDomain.CurrentDomain.BaseDirectory, configFileName);
			config = XmlCofigHelper.Load<PrintingNameToValueMappingConfig>(cofigFilePath);

			expressionDic = new Dictionary<string, Func<TicketResponseData, DateTime, string>>();
		}

		public static string AssignValue(TicketResponseData ticket, string templateXml)
		{
			var templateXmlStringBuilder = new StringBuilder(templateXml);
			var now = DateTime.Now;
			
			foreach (var mapping in config.NameToValueMappingMappings)
			{
				Func<TicketResponseData, DateTime, string> valueExpression;
				var key = GenerateExpressionDicKey(ticket, mapping.Expression);
				if (expressionDic.ContainsKey(key))
				{
					valueExpression = expressionDic[key];
				}
				else
				{
					var task = CSharpScript.EvaluateAsync<Func<TicketResponseData, DateTime, string>>(mapping.Expression, expressionOptions);
					task.Wait();
					expressionDic[key] = task.Result;
					valueExpression = expressionDic[key];
				}

				var value = valueExpression(ticket, now);
				if(value == null)
				{
					value = string.Empty;
				}
				templateXmlStringBuilder.Replace(mapping.TemplateString, value);
			}

			return templateXmlStringBuilder.ToString();
		}

		private static string GenerateExpressionDicKey(TicketResponseData ticket, string expressionString)
		{
			return $"{ticket.scheduleId}_{expressionString}";
		}
	}
}
