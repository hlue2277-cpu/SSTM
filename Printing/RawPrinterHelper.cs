using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Printing
{
    public class RawPrinterHelper
    {
        // Structure and API declarions:
        public static IntPtr Write_Printer_Handle = new IntPtr(0);
        public static IntPtr Read_Printer_Handle = new IntPtr(0);
        public static string port_name = "";
        public static int port_access = 0;
        public static int original_attributes = 0;

        //Constants
        public const int PRINTER_ACCESS_ADMINISTER = 0x00000004;
        public const int PRINTER_ALL_ACCESS = 0xf000c;
        public const int PRINTER_USE_ACCESS = 0x8;
        public const int PRINTER_STATUS_READY = 0x0;
        public const int PRINTER_STATUS_PAUSED = 0x1;
        public const int PRINTER_STATUS_ERROR = 0x2;
        public const int PRINTER_STATUS_JAM = 0x8;
        public const int PRINTER_STATUS_OUT = 0x10;
        public const int PRINTER_STATUS_OFFLINE = 0x80;
        public const int PRINTER_STATUS_PRINTING = 0x400;
        public const int PRINTER_ATTRIBUTE_QUEUED = 0x1;
        public const int PRINTER_ATTRIBUTE_DIRECT = 0x2;
        public const int PRINTER_ATTRIBUTE_DEFAULT = 0x4;
        public const int PRINTER_ATTRIBUTE_SHARED = 0x8;
        public const int PRINTER_ATTRIBUTE_NETWORK = 0x10;
        public const int PRINTER_ATTRIBUTE_HIDDEN = 0x20;
        public const int PRINTER_ATTRIBUTE_LOCAL = 0x40;
        public const int PRINTER_ATTRIBUTE_ENABLEDEVQ = 0x80;
        public const int PRINTER_ATTRIBUTE_KEEPPRINTEDJOBS = 0x100;
        public const int PRINTER_ATTRIBUTE_DO_COMPLETE_FIRST = 0x200;
        public const int PRINTER_ATTRIBUTE_WORK_OFFLINE = 0x400;
        public const int PRINTER_ATTRIBUTE_ENABLE_BIDI = 0x800;
        public const int PRINTER_ATTRIBUTE_RAW_ONLY = 0x1000;
        public const int PRINTER_ATTRIBUTE_PUBLISHED = 0x2000;
        public const int PRINTER_CONTROL_PAUSE = 1;
        public const int PRINTER_CONTROL_RESUME = 2;
        public const int PRINTER_CONTROL_PURGE = 3;
        public const int PRINTER_CONTROL_SET_STATUS = 4;


        //thread for read operations
        protected Thread dataReadingThread;
        public static RawPrinterHelper boca_raw;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class PRINTERDEFAULTS
        {
            public string pDataType;
            public int pDevMode;
            public int DesiredAccess;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public class DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDocName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDataType;
        }

        [DllImport("winspool.Drv", EntryPoint = "OpenPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool OpenPrinter([MarshalAs(UnmanagedType.LPStr)] string szPrinter, out IntPtr hPrinter, IntPtr pd);

        [DllImport("winspool.Drv", EntryPoint = "ClosePrinter", SetLastError = true, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool ClosePrinter(IntPtr hPrinter);


        [DllImport("winspool.Drv", EntryPoint = "GetPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool GetPrinter(IntPtr hPrinter, Int32 dwLevel, IntPtr pPrinter, Int32 dwBuf, out Int32 dwNeeded);

        [DllImport("winspool.Drv", EntryPoint = "SetPrinterA", SetLastError = true, CharSet = CharSet.Ansi, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        public static extern bool SetPrinter(IntPtr hPrinter, Int32 dwLevel, IntPtr pPrinter, Int32 command);

        [StructLayout(LayoutKind.Sequential)]
        private class PRINTER_INFO_2
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pServerName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPrinterName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pShareName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPortName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDriverName;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pComment;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pLocation;
            public IntPtr pDevMode;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pSepFile;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pPrintProcessor;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pDatatype;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public Int32 Attributes;
            public Int32 Priority;
            public Int32 DefaultPriority;
            public Int32 StartTime;
            public Int32 UntilTime;
            public Int32 Status;
            public Int32 cJobs;
            public Int32 AveragePPM;
        }
        public RawPrinterHelper()
        {
            boca_raw = this;
        }
        public static int Open_Printer(string szPrinterName)
        {

            IntPtr hPrinter = new IntPtr(0);
            PRINTERDEFAULTS df = new PRINTERDEFAULTS();
            int bSuccess = 0;
            bool online = false;
            //Init Printer Default Structure
            df.pDataType = "RAW";
            df.pDevMode = 0;
            df.DesiredAccess = PRINTER_ALL_ACCESS;


            IntPtr def = Marshal.AllocHGlobal(Marshal.SizeOf(df));
            Marshal.StructureToPtr(df, def, false);

            port_name = RawPrinterHelper.Get_Printer_Portname(szPrinterName, def);

            //Open the first channel to the printer using the printer name for the intent of writing raw data
            if (OpenPrinter(szPrinterName.Normalize(), out Write_Printer_Handle, def))
            {

                //indicate successful write access
                bSuccess = 1;

                online = Is_Printer_Online();           //3.0.0.0
                if (online)
                {
                    //open second channel to the printer using port name for the intent of reading return status from the printer
                    //A read thread will be started using the read handle and operate as a port monitor.  When data is returned from
                    //printer the callback function DisplayText() is used to display status on the main parent form.  See ReadData() Below.
                    if (OpenPrinter(port_name.Normalize(), out Read_Printer_Handle, def))
                    {
                        //indicate read access also
                        bSuccess = 2;
                    }

                }
                else//3.0.0.0
                {
                    //indicate no connection because printer is offline
                    bSuccess = 3;
                }
            }
            Marshal.FreeHGlobal(def);
            return bSuccess;
        }


        public static string Get_Printer_Portname(string szPrinterName, IntPtr def)
        {
            string pn = "";         //Port Name
            string sn = "";         //Server Name
            int stat = 0;
            int att = 0;

            char backslash = (char)92;
            string ReturnValue = "";
            Int32 level = 2;
            Int32 Needed = 0;
            IntPtr hPrinter = new IntPtr(0);
            IntPtr pPrinter = new IntPtr(0);
            PRINTER_INFO_2 pi = new PRINTER_INFO_2();

            // Open the printer.
            if (OpenPrinter(szPrinterName.Normalize(), out hPrinter, def))
            {
                // Get printer level page size needed - first call
                if (GetPrinter(hPrinter, level, pPrinter, 0, out Needed) == false)
                {
                    // Allocate needed memory
                    IntPtr pBytes = Marshal.AllocCoTaskMem(Needed);

                    // Get printer level data - second call
                    GetPrinter(hPrinter, level, pBytes, Needed, out Needed);

                    // Convert printer data block into class structure
                    Marshal.PtrToStructure(pBytes, pi);
                    ReturnValue = pi.pDriverName; // get printer driver name
                    sn = pi.pServerName;
                    pn = pi.pPortName;
                    stat = pi.Status;
                    att = pi.Attributes;

                    //set advanced attribute for direct print not spool on local printers only
                    if (((att & PRINTER_ATTRIBUTE_DIRECT) == 0) && (sn == null))
                    {
                        //save original values to restore later
                        original_attributes = att;

                        att |= PRINTER_ATTRIBUTE_DIRECT;
                        pi.Attributes = att;
                        Marshal.StructureToPtr(pi, pBytes, false);
                        if (!SetPrinter(hPrinter, level, pBytes, 0))
                        {
                            //throw new ApplicationException("Cannot alter attribute spool - print direct " + Marshal.GetLastWin32Error());
                            //MessageBox.Show("Cannot alter attribute spool - print direct " + Marshal.GetLastWin32Error());

                        }

                    }
                    else
                    {
                        //no original attribute values saved
                        original_attributes = 0;
                    }
                    // Free memory
                    Marshal.FreeCoTaskMem(pBytes);
                }
                ClosePrinter(hPrinter);
            }

            //check to see if the referenceds printer driver port is local to current PC or a network printer on another PC
            if (sn == null)
            {
                pn = pn + ", Port";                         //build port name
                port_access = PRINTER_ALL_ACCESS;
            }
            else
            {
                pn = sn + backslash + pn + ", Port";        //build port name with server name
                port_access = PRINTER_USE_ACCESS;
            }

            //return the port name for use in attempting to read from the printer
            return pn;
        }

        //1.1.2.1 check the printer attributes to see if printer online or offline
        public static bool Is_Printer_Online()
        {

            bool online = false;            //initialize to false
            int att = 0;
            Int32 level = 2;
            Int32 Needed = 0;
            IntPtr pPrinter = new IntPtr(0);
            PRINTER_INFO_2 pi = new PRINTER_INFO_2();

            // Get printer level page size needed - first call
            if (GetPrinter(Write_Printer_Handle, level, pPrinter, 0, out Needed) == false)
            {
                if (Needed > 0)
                {
                    // Allocate needed memory
                    IntPtr pBytes = Marshal.AllocCoTaskMem(Needed);

                    // Get printer level data - second call
                    GetPrinter(Write_Printer_Handle, level, pBytes, Needed, out Needed);

                    // Convert printer data block into class structure
                    Marshal.PtrToStructure(pBytes, pi);
                    att = pi.Attributes;

                    //check if printer online
                    if ((att & PRINTER_ATTRIBUTE_WORK_OFFLINE) == 0)
                        online = true;

                    // Free memory
                    Marshal.FreeCoTaskMem(pBytes);
                }
            }

            //1.1.2.1  check if offline close any open channels
            //if (!online)
            //{
            //    quick_close();
            //}

            return (online);
        }
    }
}
