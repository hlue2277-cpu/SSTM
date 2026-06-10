using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CtripTerminal.Helpers
{
    public static class T080Printer
    {
        #region T080 printer
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_Init();
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_FeedLIne(Int32 nNum);
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_OpenPort();
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_IsPrinterReady(int iTimeOut);
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_CutPaper();
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_ClosePort();
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_CheckJob(int iTimeOut);
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_WaitPrintJobStart(int iTimeOut);
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_PrintText(string pString, Int32 nOrgx, Int32 nOrgy, Int32 nWidthTimes, Int32 nHeightTimes);
        [DllImport("PrintCtrl.dll")]
        public static extern int WPrinter_PrintBarcode(string pInfo, Int32 nOrgx, Int32 nOrgy, Int32 nWidth, Int32 nLanguageMode, Int32 nErrorCorrect, Int32 nBytesOfInfo);
        #endregion
    }
}
