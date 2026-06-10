using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Printing
{
    public class PrinterFactory
    {
        public static PrinterBase Create(string type)
		{
            // 0:T080,1:BTP6206,2:VK80
            switch (type)
			{
                case "0":
                    return new T080Printer();
                case "1":
                    return new BTP6206Printer();
                case "2":
                    return new VK80Printer();
			}
            return null;
		}

    }
}
