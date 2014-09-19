using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OnCallScheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OnCallSchedulerTests
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class SpreadingOptimziations
    {
        private const string LogPath = @"C:\OcsaLogs\SpreadingOptimizations.txt";
        private readonly TestScheduleFactory ScheduleFactory = new TestScheduleFactory();

        /// <summary>
        /// Creates a few schedules and obtains statistics on the distribution of agents.
        /// </summary>
        [TestMethod]
        public void GetStatistics()
        {
            File.AppendAllText(LogPath, DateTime.Now + ": Beginning new batch." + Environment.NewLine);
            var batchCount = 1000;
            decimal averageSpread = 0;
            var minSpreaed = 0.0;
            var maxDisc = 0.0;
            for (int i = 0; i < batchCount; i++)
            {
                var schedule = ScheduleFactory.createScheduleFromAgentsTxt();
                averageSpread += schedule.AverageSpread;
                minSpreaed += schedule.MinimumSpread;
                maxDisc += schedule.MaxPointDiscrepancy;
            }


            var lines = new[]
                {
                    "    Average Spread: " + averageSpread / batchCount,
                    "    Minimum Spread: " + minSpreaed / batchCount,
                    "    MaxDiscrepancy: " + maxDisc / batchCount
                };
            File.AppendAllLines(LogPath, lines);
        }

        [TestMethod]
        public void AverageSpread()
        {
            var agentTwo = new Agent { Name = "Two" };
            var agentThree = new Agent { Name = "Three" };
            var agentFiller = new Agent { Name = "Filler" };
            var agents = new List<Agent> {agentTwo ,agentThree, agentFiller  };
            var schedule = new Schedule(agents, DateTime.Now, DateTime.Now.AddDays(5));
            schedule.AssignPrimary(schedule[0], agentTwo);
            schedule.AssignPrimary(schedule[1], agentThree);
            schedule.AssignPrimary(schedule[2], agentTwo);
            schedule.AssignPrimary(schedule[3], agentFiller);
            schedule.AssignPrimary(schedule[4], agentThree);
            Assert.AreEqual((decimal)2.5, schedule.AverageSpread);

        }
    }
}
