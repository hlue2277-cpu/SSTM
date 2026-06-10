namespace DataService
{
    /// <summary>
    /// 返回信息
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseModel<T>
    {
        public bool success { get; set; }
        public string msg { get; set; }
        public string errorcode { get; set; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public T data { get; set; }
    }
}