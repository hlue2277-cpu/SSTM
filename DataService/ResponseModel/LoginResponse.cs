namespace DataService
{
    public class LoginResponse
    {
        public bool success { get; set; }
        public LoginResponseData data { get; set; }
    }


    public class LoginResponseData
    {
        public string rolenames { get; set; }
        public string realname { get; set; }
        public string id {  get; set; }
        public string memberEncode { get; set; }
    }
}