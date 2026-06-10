using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Printing
{
	[Serializable]
	[XmlRoot(ElementName = "PrintingNameToValueMappingConfig")]
	public class PrintingNameToValueMappingConfig
	{
		[XmlArray]
		[XmlArrayItem("NameToValueMapping")]
		public List<NameToValueMapping> NameToValueMappingMappings { get; set; }
	}

	[Serializable]
	public class NameToValueMapping
	{
		public string TemplateString { get; set; }
		public string Expression { get; set; }
	}
}
