using Genesis.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal
{
    public class StartupEvent : CompositePresentationEvent<object>
    {
    }

    public class ClosingEvent : CompositePresentationEvent<object>
    {
    }
}
