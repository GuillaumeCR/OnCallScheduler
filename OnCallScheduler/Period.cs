using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnCallScheduler
{
    public class Period
    {
        public DateTime Day { get; set; }
        public Agent Primary { get; set; }
        //public Agent Backup { get; set; }

        public int PointValue
        {
            get
            {
                if (Day.DayOfWeek == DayOfWeek.Saturday || Day.DayOfWeek == DayOfWeek.Sunday)
                {
                    return 2;
                }
                return 1;
            }
        }
    }
}
