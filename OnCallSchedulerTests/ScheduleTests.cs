using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OnCallScheduler;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using OnCallSchedulerTests.Properties;

namespace OnCallSchedulerTests
{
    [TestClass]
    public class ScheduleTests
    {
        [TestMethod]
        public void CreatesPeriods()
        {
            var schedule = createSchedule();
            Assert.AreEqual(2, schedule.Count(), "Did not create 2 periods.");
        }

        [TestMethod]
        public void Shuffles()
        {
            var first = createScheduleFromAgentsTxt();
            var succeededOnce = false;
            //Should get one success in 5 shots at least.
            for (int i = 0; i < 5; i++)
            {
                //Sleep to change Rand seed. tsk tsk tsk.
                System.Threading.Thread.Sleep(10);
                var second = createScheduleFromAgentsTxt();
                //Check the first 3 primaries. At least 1 of those should be different.
                for (int j = 0; j < 3; j++)
                {
                    Assert.IsNotNull(first[j].Primary);
                    Assert.IsNotNull(second[j].Primary);
                    if (first[j].Primary.Name != second[j].Primary.Name)
                    {
                        succeededOnce = true;
                        break;
                    }
                }
                if (succeededOnce)
                {
                    break;
                }
            }
            Assert.IsTrue(succeededOnce, "Two consecutive schedules had the same first primary.");
        }

        private Schedule createSchedule(List<Agent> agents = null, int days = 2)
        {
            if (agents == null)
            {
                agents = new List<Agent> { 
                        new Agent { Name = "Primary" }, 
                        new Agent { Name = "Backup" } };
            }
            return new Schedule(agents,
                    DateTime.Now,
                    DateTime.Now + new TimeSpan(days, 0, 0, 0));
        }

        [TestMethod]
        public void AlernatesPrimaries()
        {
            var schedule = createSchedule();
            Assert.AreNotEqual(schedule[0].Primary, schedule[1].Primary);
        }

        private Schedule createScheduleFromAgentsTxt()
        {
            var agents = JsonConvert.DeserializeObject<List<Agent>>(Resources.AgentsJson);
            var start = new DateTime(2014, 7, 28);
            return new Schedule(agents, start, start + new TimeSpan(28, 0, 0, 0));
        }

        [TestMethod]
        public void RespectsSimpleCantWorkOn()
        {
            var schedule = createScheduleFromAgentsTxt();
            foreach (var period in schedule)
            {
                foreach (var vacation in period.Primary.CantWorkOn)
                {
                    if (period.Day.Date == vacation.Date)
                    {
                        Assert.Fail("Agent " + period.Primary.Name + " was assigned on " + period.Day.Date.ToShortDateString() + " even though s/he is on vacation that day.");
                    }
                }
            }
            assertMinimumAssignment(schedule);
        }

        private static void assertMinimumAssignment(Schedule schedule)
        {
            var assigned = 0;
            foreach (var period in schedule)
            {
                if (period.Primary != null)
                {
                    assigned += 1;
                }
            }
            Assert.IsTrue(assigned >= schedule.Count * 3 / 4, "Less than 75% of the schedule was filled up");
        }

        [TestMethod]
        public void NotPrimaryTwoDaysInARow()
        {
            var schedule = createScheduleFromAgentsTxt();
            Agent previousAgent = null;
            foreach (var period in schedule)
            {
                if (previousAgent == period.Primary)
                {
                    Assert.Fail("Assigned " + previousAgent.Name + " two days in a row on " + period.Day.ToShortDateString());
                }
                previousAgent = period.Primary;
            }

            assertMinimumAssignment(schedule);
        }
    }
}
