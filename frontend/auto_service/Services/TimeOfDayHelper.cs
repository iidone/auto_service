using System;

namespace Auto_Service.Services;

public static class TimeOfDayHelper
{
    public static string GetTimeOfDay()
    {
        var hour = DateTime.Now.Hour;

        return hour switch
        {
            >= 5 and < 17 => "день",
            >= 17 and < 23 => "вечер",
            _ => "ночь"
        };
    }
}