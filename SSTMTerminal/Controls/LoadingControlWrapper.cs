using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal.Controls
{
    public class LoadingControlWrapper
    {
        public LoadingControlManager Instance
        {
            get { return LoadingControlManager.Instance; }
        }
    }
}
