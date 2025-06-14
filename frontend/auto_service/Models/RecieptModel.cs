using System;
using System.Collections.Generic;

namespace Auto_Service.Models;

public record MaintenanceRequest(
    int Id,
    string ClientName,
    string Car,
    string Description,
    string? TelegramChatId,
    List<SparePart> UsedParts,
    decimal TotalPrice,
    DateTime Date);

public record SparePart(
    int Id,
    string Name,
    decimal Price,
    int Quantity = 1);
