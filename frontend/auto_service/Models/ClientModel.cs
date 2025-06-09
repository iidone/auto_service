namespace Auto_Service.Models;

public class ClientModel
{
    public int id {get; set;}
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string contact { get; set; }
    public string brand { get; set; }
    public string series { get; set; }
    public string number { get; set; }
    public string mileage { get; set; }
    public string age { get; set; }
    public string vin { get; set; }
    public string last_maintenance { get; set; }
    public string ShortName => $"{last_name} {first_name?[0]}.";
}