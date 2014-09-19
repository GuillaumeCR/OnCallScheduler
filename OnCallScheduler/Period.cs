using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnCallScheduler
{
    public class Period
    {
        public Period()
        {
            AvailableAgents = new List<Agent>();
        }

        public DateTime Day { get; set; }
        public Agent Primary { get; set; }
        public List<Agent> AvailableAgents { get; set; }

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

        public override string ToString()
        {
            return "Day: " + Day.ToString("MM/dd") + ", Primary: " + Primary.Name + ", PointValue: " + PointValue;
        }
    }
}
