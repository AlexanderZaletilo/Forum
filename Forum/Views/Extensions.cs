using System;
using Microsoft.AspNetCore.Http;

namespace Forum.Views
{
    public static class Extensions
    {
        public static string ToClientTime(this DateTime dt, ISession session)
        {
            var timeOffSet = session.GetInt32("timezoneoffset");  

            if (timeOffSet != null)
            {
                var offset = int.Parse(timeOffSet.ToString());
                dt = dt.AddMinutes(-1 * offset);
                return dt.ToString();
            }
            return dt.ToLocalTime().ToString();
        }
    }
}
