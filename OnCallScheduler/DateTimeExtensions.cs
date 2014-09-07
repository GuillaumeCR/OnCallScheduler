using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnCallScheduler
{
    public static class DateTimeExtensions
    {
        public static bool IsWeekEnd(this DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Friday ||
                date.DayOfWeek == DayOfWeek.Saturday ||
                date.DayOfWeek == DayOfWeek.Sunday;
        }

        public static DateTime[] GetLastWeekEnd(this DateTime date)
        {
            if (date.IsWeekEnd())
            {
                date = date.AddDays(-3);
            }

            while (date.DayOfWeek != DayOfWeek.Sunday)
            {
                date = date.AddDays(-1);
            }

            return new[] { date.AddDays(-2), date.AddDays(-1), date };
        }
    }
}
