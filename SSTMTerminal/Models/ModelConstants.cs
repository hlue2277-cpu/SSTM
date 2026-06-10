using Genesis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSTMTerminal.Models
{
    public class ModelConstants
    {
        public const int LengthOfValidID = 18;
        public const int LengthOfValidCellNo = 11;
        public const int LengthOfValidOrderNo = 4;
    }

    public class CertTypes
    {
        public const string IDCard = "idcard";
        public const string Passport = "passport";
        public const string Residence = "residence";
        public const string ForeignIdcard = "foreignIdcard";
    }

    public class PayMethods
    {
        public const string WeChatPay = "WeChatPay";
        public const string AliPay = "AliPay";
        public static string GetPayMethodString(string payMethod)
        {
            switch (payMethod)
            {
                case AliPay:
                    return "支付宝";
                case WeChatPay:
                    return "微信支付";
                default:
                    return "其他";
            }
        }
    }
}
