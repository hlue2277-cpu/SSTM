namespace DataService
{
    /// <summary>
    /// 请求扩展信息
    /// </summary>
    public class RequestExtension
    {
        public string[] extension { get; set; }

        public RequestExtension()
        {
            extension = new string[0];
        }
    }
}