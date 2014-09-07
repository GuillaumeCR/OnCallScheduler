using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OnCallScheduler
{
    public class Schedule : List<Period>
    {
        private readonly List<Agent> _agents;

        public Schedule(IEnumerable<Agent> agents, DateTime start, DateTime end)
        {
            validateArguments(agents, start, end);

            createPeriods(start, end);
            _agents = agents.ToList();
            shuffleAgents();

            assignPrimaries();
            //assignBackups();
        }

        private void createPeriods(DateTime start, DateTime end)
        {
            var periodCount = (end - start).Days;
            if (periodCount < 1)
            {
                throw new ArgumentException(string.Format("The specified start ({0}) and end ({1}) dates don't add up to a single day of schedule.", start, end));
            }
            for (int i = 0; i < periodCount; i++)
            {
                Add(new Period { Day = start + new TimeSpan(i, 0, 0, 0) });
            }
        }

        private void shuffleAgents()
        {
            _agents.Sort(new Agent.LeastPrimary());
            var start = 0;
            for (int i = 0; i < _agents.Count; i++)
            {
                if (i + 1 == _agents.Count)
                {
                    if (start != i)
                    {
                        _agents.Shuffle(start, i);
                    }
                    break;
                }
                if (_agents[i + 1].PointReduction == _agents[i].PointReduction)
                {
                    continue;
                }
                _agents.Shuffle(start, i);
                start = i;
            }
        }

        private void validateArguments(IEnumerable<Agent> agents, DateTime start, DateTime end)
        {
            if (agents == null || agents.Count() < 2)
            {
                throw new ArgumentNullException("agents");
            }
            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            if (end == null)
            {
                throw new ArgumentNullException("end");
            }
        }

        private void assignPrimaries()
        {
            Agent previousPrimary = null;
            foreach (var period in this)
            {
                Agent available = null;
                foreach (var agent in _agents)
                {
                    if (agent.IsAvailable(period.Day) && agent != previousPrimary)
                    {
                        available = agent;
                        break;
                    }
                }
                if (available != null)
                {
                    assignPrimary(period, available);
                    previousPrimary = available;
                }
            }
        }

        private void assignPrimary(Period period, Agent agent)
        {
            period.Primary = agent;
            agent.PrimaryCount += period.PointValue;

            //Move the agent to the highest index in the list above other agents with the same PrimaryPoints.
            _agents.Remove(agent);
            for (int i = 0; i < _agents.Count; i++)
            {
                if (_agents[i].PrimaryPoints > agent.PrimaryPoints)
                {
                    _agents.Insert(i, agent);
                    break;
                }
            }
            if (!_agents.Contains(agent))
            {
                _agents.Add(agent);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Schedule:");
            foreach (var period in this)
            {
                sb.AppendLine(period.Day.ToString("MM/dd") + " Primary: " + period.Primary.Name);
            }
            sb.AppendLine("Agents:");
            var jsonSettings = new JsonSerializerSettings { DateFormatString = "MM/dd" };
            foreach (var agent in _agents)
            {
                sb.AppendLine(JsonConvert.SerializeObject(agent, jsonSettings));
            }
            return sb.ToString();
        }

        //private void assignBackups()
        //{
        //    foreach (var period in this)
        //    {
        //        period.Backup = _agents[0];
        //    }
        //}
    }
}
