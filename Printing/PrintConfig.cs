using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Printing
{
	[Serializable]
	[XmlRoot(ElementName = "PrintConfig")]
	public class PrintConfig
	{
		[XmlArray]
		[XmlArrayItem("PrintSettingConfig")]
		public List<PrintSettingConfig> PrintSettingConfigs { get; set; }
	}

	public class PrintSettingConfig
	{
		public string PrinterType { get; set; }
		public string LeftMargin { get; set; }
		public string TopMargin { get; set; }
		public string Scale { get; set; }
	}
}
