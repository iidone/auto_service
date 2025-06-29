﻿namespace Auto_Service.Models;

public class MasterModel
{
    public int id { get; set; }
    public string role { get; set; }
    public string first_name { get; set; }
    public string last_name { get; set; }
    public string contact { get; set; }
    public string username {get; set;}
    public string password { get; set; }
    public string ShortName => $"{last_name} {first_name?[0]}.";
    
}