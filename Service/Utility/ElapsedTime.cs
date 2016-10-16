using System;

namespace Service.Utility
{
    public class ElapsedTime
    {
        public static string GetElapsedTime(TimeSpan timeSpan)
        {
            return $"{timeSpan.Hours:00}:{timeSpan.Minutes:00}:{timeSpan.Seconds:00}.{timeSpan.Milliseconds / 10:00}";
        }
    }
}
