using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Printing
{
    public class PrintModel
    {
        public int PrintIndex { get; set; }
        public string ToBePrintedID { get; set; }
        public string ToBeNotifiedID { get; set; }

        public XDocument XDocument { get; set; }
        public Dictionary<string, string> ImageSetting { get; set; }
    }
}
