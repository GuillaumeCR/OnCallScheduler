using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnCallScheduler;
using Newtonsoft.Json;
using OnCallSchedulerTests.Properties;

namespace OnCallSchedulerTests
{
    class TestScheduleFactory
    {
        public Schedule createScheduleFromAgentsTxt()
        {
            var agents = JsonConvert.DeserializeObject<List<Agent>>(Resources.AgentsJson);
            var start = new DateTime(2014, 7, 28);
            var schedule = new Schedule(agents, start, start + new TimeSpan(28, 0, 0, 0));
            schedule.FillUp();
            return schedule;
        }
    }
}
