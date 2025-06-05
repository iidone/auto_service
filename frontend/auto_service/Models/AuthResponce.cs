namespace Auto_Service.Models;

public class AuthResponce
{
    public string access_token { get; set; }
    public string token_type { get; set; }
    public  UserInfo user_info { get; set; }
    public string Error { get; set; }
}

public class UserInfo
{
    public string username { get; set; }
    public int user_id { get; set; }
    public string user_role { get; set; }
}