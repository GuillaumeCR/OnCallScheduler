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
        }

        public void FillUp()
        {
            shuffleAgents();

            assignPrimaries();
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

        /// <summary>
        /// Sorts agents by PrimaryPoints, then shuffles each section that has the same PrimaryPoints.
        /// </summary>
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
            for (int i = 0; i < this.Count(); i++)
            {
                var period = this[i];

                if (period.Primary != null)
                {
                    continue;
                }

                foreach (var agent in _agents)
                {
                    if (agent.IsAvailable(period.Day)
                        && (i == 0 || this[i - 1].Primary != agent) //Didn't work yesterday
                        && (i == this.Count() - 1 || this[i + 1].Primary != agent) //Isn't working tomorrow
                        && (!period.Day.IsWeekEnd() || !agentWorkedLastWeekEnd(agent, period)))
                    {
                        assignPrimary(period, agent);
                        break;
                    }
                }
            }
        }

        private bool agentWorkedLastWeekEnd(Agent agent, Period period)
        {
            var lastWeekEnd = period.Day.GetLastWeekEnd();
            foreach (var day in lastWeekEnd)
            {
                if (Contains(day) &&
                    this[day].Primary == agent)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(DateTime date)
        {
            foreach (var period in this)
            {
                if (period.Day == date)
                {
                    return true;
                }
            }
            return false;
        }

        public Period this[DateTime date]
        {
            get
            {
                foreach (var period in this)
                {
                    if (period.Day == date)
                    {
                        return period;
                    }
                }
                throw new IndexOutOfRangeException("No periods for that day.");
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
                var primaryString = "Could not assign.";
                if (period.Primary != null)
                {
                    primaryString = period.Primary.Name;
                }
                sb.AppendLine(period.Day.ToString("MM/dd") + " Primary: " + primaryString);
            }
            sb.AppendLine("Agents:");
            var jsonSettings = new JsonSerializerSettings { DateFormatString = "MM/dd" };
            foreach (var agent in _agents)
            {
                sb.AppendLine(JsonConvert.SerializeObject(agent, jsonSettings));
            }
            return sb.ToString();
        }
    }
}
