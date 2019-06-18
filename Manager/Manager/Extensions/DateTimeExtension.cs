using System;

namespace Manager.Extensions
{
    public static class DateTimeExtension
    {
        public static string GetDateWithoutTime(this DateTime value)
        {
            return value.Day + "."+value.Month+"."+value.Year;
        }
    }
}