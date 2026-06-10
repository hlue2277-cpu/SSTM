namespace DataService
{
    /// <summary>
    /// 返回结果
    /// </summary>
    public class ResponseHead
    {
        /// <summary>
        /// 错误码
        /// 0成功
        /// >0失败
        /// 1001:基本错误(验证信息错误)
        /// 1002:传参错误
        /// 1007:接口异常(系统异常)
        /// 1008:服务超时导致,无数据
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 错误提示(>0会返回错误提示)
        /// </summary>
        public string errmsg { get; set; }
    }
}