using System;

namespace SSTMTerminal.Models
{
    public class OrderReprintModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 携程订单号（后期身份证取票也许没有订单号）
        /// </summary>
        public long? ctriporderid { get; set; }

        /// <summary>
        /// 订单成交时间
        /// </summary>
        public DateTime dealtime { get; set; }

        /// <summary>
        /// 资源ID
        /// </summary>
        public long resourceid { get; set; }

        /// <summary>
        /// 资源名称
        /// </summary>
        public string resourcename { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal price { get; set; }

        /// <summary>
        /// 份数
        /// </summary>
        public int quantity { get; set; }

        /// <summary>
        /// 已打印次数
        /// </summary>
        public int printedCount { get; set; }

        /// <summary>
        /// 打印时间
        /// </summary>
        public DateTime? printedtime { get; set; }
    }
}