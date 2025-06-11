using System.Collections.Generic;

namespace Auto_Service.Models;

public class DeletePartRequest
{
    public List<int> partIds { get; set; }
}