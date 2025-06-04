namespace Auto_Service.Models;

public class AuthResponce
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public UserInfo UserInfo { get; set; }
    public string Error { get; set; }
}

public class UserInfo
{
    public string Username { get; set; }
    public int UserId { get; set; }
}