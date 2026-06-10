using System.Runtime.InteropServices;
using System.Text;

namespace Printing
{
    public class TKIOSK
    {
        public const int TKIOSK_SUCCESS = 1001;

        public const int TKIOSK_FAIL = 1002;

        public const int TKIOSK_ERROR_INVALID_HANDLE = 1101;

        public const int TKIOSK_ERROR_INVALID_PARAMETER = 1102;

        public const int TKIOSK_ERROR_INVALID_PATH = 1201;

        public const int TKIOSK_ERROR_NOT_BITMAP = 1202;

        public const int TKIOSK_ERROR_NOT_MONO_BITMAP = 1203;

        public const int TKIOSK_ERROR_BEYOND_AREA = 1204;

        public const int TKIOSK_COM = 0;

        public const int TKIOSK_LPT = 1;

        public const int TKIOSK_USB = 2;

        public const int TKIOSK_DRV = 3;

        public const int TKIOSK_NIBBLEPAR = 4;

        public const int TKIOSK_COM_ONESTOPBIT = 0;

        public const int TKIOSK_COM_TWOSTOPBITS = 1;

        public const int TKIOSK_COM_NOPARITY = 0;

        public const int TKIOSK_COM_ODDPARITY = 1;

        public const int TKIOSK_COM_EVENPARITY = 2;

        public const int TKIOSK_COM_MARKPARITY = 3;

        public const int TKIOSK_COM_SPACEPARITY = 4;

        public const int TKIOSK_COM_DTR_DSR = 0;

        public const int TKIOSK_COM_RTS_CTS = 1;

        public const int TKIOSK_COM_XON_XOFF = 2;

        public const int TKIOSK_COM_NO_HANDSHAKE = 3;

        public const int TKIOSK_PRINT_MODE_STANDARD = 0;

        public const int TKIOSK_PRINT_MODE_PAGE = 1;

        public const int TKIOSK_PAPER_SERIAL = 0;

        public const int TKIOSK_PAPER_SIGN = 1;

        public const int TKIOSK_FONT_TYPE_STANDARD = 0;

        public const int TKIOSK_FONT_TYPE_COMPRESSED = 1;

        public const int TKIOSK_FONT_TYPE_UDC = 2;

        public const int TKIOSK_FONT_TYPE_CHINESE = 3;

        public const int TKIOSK_FONT_STYLE_NORMAL = 0;

        public const int TKIOSK_FONT_STYLE_BOLD = 8;

        public const int TKIOSK_FONT_STYLE_THIN_UNDERLINE = 128;

        public const int TKIOSK_FONT_STYLE_THICK_UNDERLINE = 256;

        public const int TKIOSK_FONT_STYLE_UPSIDEDOWN = 512;

        public const int TKIOSK_FONT_STYLE_REVERSE = 1024;

        public const int TKIOSK_FONT_STYLE_CLOCKWISE_90 = 4096;

        public const int TKIOSK_BITMAP_MODE_8SINGLE_DENSITY = 0;

        public const int TKIOSK_BITMAP_MODE_8DOUBLE_DENSITY = 1;

        public const int TKIOSK_BITMAP_MODE_24SINGLE_DENSITY = 32;

        public const int TKIOSK_BITMAP_MODE_24DOUBLE_DENSITY = 33;

        public const int TKIOSK_BITMAP_PRINT_NORMAL = 0;

        public const int TKIOSK_BITMAP_PRINT_DOUBLE_WIDTH = 1;

        public const int TKIOSK_BITMAP_PRINT_DOUBLE_HEIGHT = 2;

        public const int TKIOSK_BITMAP_PRINT_QUADRUPLE = 3;

        public const int TKIOSK_BARCODE_TYPE_UPC_A = 65;

        public const int TKIOSK_BARCODE_TYPE_UPC_E = 66;

        public const int TKIOSK_BARCODE_TYPE_JAN13 = 67;

        public const int TKIOSK_BARCODE_TYPE_JAN8 = 68;

        public const int TKIOSK_BARCODE_TYPE_CODE39 = 69;

        public const int TKIOSK_BARCODE_TYPE_ITF = 70;

        public const int TKIOSK_BARCODE_TYPE_CODEBAR = 71;

        public const int TKIOSK_BARCODE_TYPE_CODE93 = 72;

        public const int TKIOSK_BARCODE_TYPE_CODE128 = 73;

        public const int TKIOSK_HRI_POSITION_NONE = 0;

        public const int TKIOSK_HRI_POSITION_ABOVE = 1;

        public const int TKIOSK_HRI_POSITION_BELOW = 2;

        public const int TKIOSK_HRI_POSITION_BOTH = 3;

        public const int TKIOSK_AREA_LEFT_TO_RIGHT = 0;

        public const int TKIOSK_AREA_BOTTOM_TO_TOP = 1;

        public const int TKIOSK_AREA_RIGHT_TO_LEFT = 2;

        public const int TKIOSK_AREA_TOP_TO_BOTTOM = 3;

        private static int hPort = -1;

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_OpenCom(string lpName, int nComBaudrate, int nComDataBits, int nComStopBits, int nComParity, int nFlowControl);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetComTimeOuts(int hPort, int nWriteTimeoutMul, int nWriteTimeoutCon, int nReadTimeoutMul, int nReadTimeoutCon);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_CloseCom(int hPort);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_OpenLptByDrv(ushort LPTAddress);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_CloseDrvLPT(int nPortType);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_OpenUsb();

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_OpenUsbByID(int nID);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_CloseUsb(int hPort);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetUsbTimeOuts(int hPort, ushort wReadTimeouts, ushort wWriteTimeouts);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_EnumDrvPrinter(StringBuilder Drivername, ref int Number, string KeyWord);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_OpenDrv(char[] drivername);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_CloseDrv(int hPort);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_StartDoc(int hPort);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_EndDoc(int hPort);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_OpenNibblePar(int PortNumber, int DeviceNumber, string DriverPath);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetNibbleParTimeOuts(int hPort, ushort wReadTimeouts, ushort wWriteTimeouts);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_NibbleParPrintToMemory();

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_NibbleParFlushMemory();

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_CloseNibblePar();

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_WriteData(int hPort, int nPortType, char[] pszData, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_ReadData(int hPort, int nPortType, int nStatus, StringBuilder pszBuffer, int nBytesToRead);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SendFile(int hPort, int nPortType, string filename);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_BeginSaveToFile(int hPort, int nPortType, char[] pszData, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_EndSaveToFile(int hPort, int nPortType, char[] pszData, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_QueryStatus(int hPort, int nPortType, char[] pszData, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_RTQueryStatus(int hPort, int nPortType, StringBuilder pszData);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_QueryASB(int hPort, int nPortType, char[] pszData, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_S_TestPrint(int hPort, int nPortType, char[] pszData, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetPrintFromStart(int hPort, int nPortType, char[] pszData, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetRaster(int hPort, int nPortType, string pszData, int nTranslateMode, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetChineseFont(int hPort, int nPortType, char[] pszData, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_GetVersionInfo(int hPort, int nPortType, char[] pszData, int nBytesToWrite);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_Reset(int hPort, int nPortType);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetPaperMode(int hPort, int nPortType, int nMode);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetMode(int hPort, int nPortType, int nPrintMode);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_S_SetLeftMarginAndAreaWidth(int hPort, int nPortType, int nDistance, int nWidth);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetCharSetAndCodePage(int hPort, int nPortType, int nCharSet, int nCodePage);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetOppositePosition(int hPort, int nPortType, int nPrintMode, int nHorizontalDist, int nVerticalDist);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetTabs(int hPort, int nPortType, string pszPosition, int nCount);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_ExecuteTabs(int hPort, int nPortType);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_FeedLine(int hPort, int nPortType);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_FeedLines(int hPort, int nPortType, int nLines);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_CutPaper(int hPort, int nPortType, int nMode, int nDistance);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_MarkerFeedPaper(int hPort, int nPortType);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetLineSpacing(int hPort, int nPortType, int nDistance);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_SetRightSpacing(int hPort, int nPortType, int nDistance);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_S_SetAlignMode(int hPort, int nPortType, int nMode);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_S_Textout(int hPort, int nPortType, string pszData, int nOrgx, int nWidthTimes, int nHeightTimes, int nFontType, int nFontStyle);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_S_PrintBarcode(int hPort, int nPortType, string pszBuffer, int nOrgx, int nType, int nWidth, int nHeight, int nHriFontType, int nHriFontPosition, int nBytesOfBuffer);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_PreDownloadBmpToRAM(int hPort, int nPortType, string pszPaths, int nTranslateMode, int nID);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_PreDownloadBmpToFlash(int hPort, int nPortType, string[] pszPaths, int nTranslateMode, int nCount);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_S_DownloadPrintBmp(int hPort, int nPortType, string pszPath, int nOrgx, int nMode);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_S_PrintBmpInRAM(int hPort, int nPortType, int nID, int nOrgx, int nMode);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_S_PrintBmpInFlash(int hPort, int nPortType, int nID, int nOrgx, int nMode);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_S_DownloadPrintBmp(int hPort, int nPortType, string pszPath, int nTranslateMode, int nOrgx, int nMode);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_P_SetAreaAndDirection(int hPort, int nPortType, int nOrgx, int nOrgy, int nWidth, int nHeight, int nDirection);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_P_Print(int hPort, int nPortType);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_P_Clear(int hPort, int nPortType);

        [DllImport("KIOSKDLL.dll")]
        public static extern int TKIOSK_P_Textout(int hPort, int nPortType, string pszData, int nOrgx, int nOrgy, int nWidthTimes, int nHeightTimes, int nFontType, int nFontStyle);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_P_PrintBarcode(int hPort, int nPortType, char[] pszBuffer, int nOrgx, int nOrgy, int nType, int nWidth, int nHeight, int nHriFontType, int nHriFontPosition, int nBytesOfBuffer);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_P_PrintBmpInRAM(int hPort, int nPortType, int nID, int nOrgx, int nOrgy, int nMode);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_P_PrintBmpInFlash(int hPort, int nPortType, int nID, int nOrgx, int nOrgy, int nMode);

        [DllImport("TKIOSKDLL.dll")]
        public static extern int TKIOSK_P_DownloadPrintBmp(int hPort, int nPortType, string pszPath, int nOrgx, int nOrgy, int nMode);

        public static bool OpenPort(out string msg)
        {
            bool flag = false;
            msg = "";
            hPort = TKIOSK_OpenUsb();
            if (hPort == -1)
            {
                msg = "Open USB failed:" + hPort;
            }
            else
            {
                flag = true;
                msg = "Open USB success!";
            }

            return flag;
        }

        public static bool ClosePort(out string msg)
        {
            bool flag = false;
            msg = "";
            if (hPort != -1)
            {
                try
                {
                    var num = TKIOSK_CloseUsb(hPort);
                    if (num == 1001)
                    {
                        msg = "关闭端口成功";
                        flag = true;
                    }
                    else
                    {
                        msg = "关闭端口失败:" + num;
                    }
                }
                catch (System.Exception ex)
                {
                    msg = "关闭端口失败:" + ex.Message;
                }
            }

            return flag;
        }

        public static StatusEnum GetStatus(out string msg)
        {
            StatusEnum result = StatusEnum.Null;
            msg = "";
            int nPortType = 2;
            StringBuilder stringBuilder = new StringBuilder();
            int num = 1002;
            int[] statusBit = new int[8] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
            char[] currentStatus = new char[8];

            num = TKIOSK_RTQueryStatus(hPort, nPortType, stringBuilder);
            if (num != 1001)
            {
                msg = "Query Status Failed :" + num + "," + stringBuilder;
                result = StatusEnum.QueryFail;
            }
            else
            {
                try
                {
                    stringBuilder.CopyTo(0, currentStatus, 0, stringBuilder.Length);
                    for (int i = 0; i < 8; i++)
                    {
                        statusBit[i] = (currentStatus[0] >> i) & 0x01;
                    }

                    if (currentStatus[0] == '\0')
                    {
                        msg = "All is OK.";
                        result = StatusEnum.Ok;
                    }
                    else
                    {
                        if (statusBit[0] == 1)
                        {
                            msg = "Head Up 上盖打开!";
                            result = StatusEnum.HeadUp;
                        }
                        else if (statusBit[1] == 1)
                        {
                            msg = "Paper End 纸尽!";
                            result = StatusEnum.PaperEnd;
                        }
                        else if (statusBit[2] == 1)
                        {
                            msg = "Cutter Error 切刀错误!";
                            result = StatusEnum.CutterError;
                        }
                        else if (statusBit[3] == 1)
                        {
                            msg = "TPH Too Hot 打印头过热!";
                            result = StatusEnum.TPHTooHot;
                        }
                        else if (statusBit[4] == 1)
                        {
                            msg = "Paper Near End 纸将尽!";
                            result = StatusEnum.PaperNearEnd;
                        }
                        else if (statusBit[7] == 1)
                        {
                            msg = "Paper Jam 卡纸!";
                            result = StatusEnum.PaperJam;
                        }

                        msg += stringBuilder.ToString();
                    }
                }
                catch (System.Exception ex)
                {
                    msg = $"设备返回值：{stringBuilder.ToString()},异常信息:{ex.Message}";
                }
                finally
                {
                    stringBuilder.Remove(0, stringBuilder.Length);
                    currentStatus[0] = '\0';
                }
            }

            return result;
        }
    }

    public enum StatusEnum
    {
        Null = -1,
        QueryFail,
        Ok,
        HeadUp,
        PaperEnd,
        CutterError,
        TPHTooHot,
        PaperNearEnd,
        PaperJam
    }
}