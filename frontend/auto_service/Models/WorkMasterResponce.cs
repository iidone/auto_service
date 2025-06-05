using System.Security;

namespace Auto_Service.Models;

public class WorkMasterResponce
{
    public maintenance Maintenance {
        get;
        set;
    }

    public client Client { 
        get;
        set;
    }
    public string Error {get;set;}


    public class maintenance
    {
        public int user_id { get; set; }
        public int client_id { get; set; }
        public string desctiption { get; set; }
        public string date  { get; set; }
        public string next_maintenance { get; set; }
        public string comment { get; set; }
        public string status { get; set; }
        public string price  { get; set; }
    }

    public class client
    {
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string contact {get; set;}
        public string brand { get; set; }
        public string series { get; set; }
        public string number { get; set; }
        public string mileage { get; set; }
        public string age  { get; set; }
        public string vin { get; set; }
        public string last_maintenance { get; set; }
    }
}