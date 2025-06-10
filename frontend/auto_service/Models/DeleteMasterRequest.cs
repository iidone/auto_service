using System.Collections.Generic;

namespace Auto_Service.Models;

public class DeleteMasterRequest
{
    public List<int> Ids { get; set; } = new List<int>();
}