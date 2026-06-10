namespace DataService
{
    /// <summary>
    /// 请求基类
    /// </summary>
    public abstract class RequestBase
    {
        /// <summary>
        /// 扩展信息
        /// </summary>
        public RequestExtension head { get; set; }

        public RequestBase()
        {
            head = new RequestExtension();
        }
    }
}