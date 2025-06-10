namespace Auto_Service.Models;

public class AddMaintenanceRequest
{
    public int user_id { get; set; }
    public int client_id { get; set; }
    public string description { get; set; }
    public string date { get; set; }
    public string next_maintenance  { get; set; }
    public string comment { get; set; }
    public string status { get; set; }
    public string price { get; set; }
}