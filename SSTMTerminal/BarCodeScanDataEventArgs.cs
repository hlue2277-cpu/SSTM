using Genesis.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal
{
    public class BarCodeScanDataEventArgs : EventArgs
    {
        public Guid RequestGuid { get; set; }
        public string ScannedData { get; set; }
    }
}
