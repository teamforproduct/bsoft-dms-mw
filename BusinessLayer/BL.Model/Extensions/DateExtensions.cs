using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.Extensions
{
    public static class DateExtensions
    {

        public static DateTime? ToUTC(this DateTime? date) => date.HasValue ? ToUTC(date.Value) : date;

        public static DateTime ToUTC(this DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified)
                return DateTime.SpecifyKind(date, DateTimeKind.Utc);
            else
                return date.ToUniversalTime();//DateTime.SpecifyKind(date, DateTimeKind.Utc);
        }

        public static DateTime Add1000Years(this DateTime dayr) => DateTime.UtcNow.AddYears(1000);

    }

}
