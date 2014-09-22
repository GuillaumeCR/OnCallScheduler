using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace OnCallScheduler
{
    public enum ScheduleFillingStrategy
    {
        Chronological,
        ByLeastAvailablesFirst
    }

    public class Schedule : List<Period>
    {
        /// <summary>
        /// The point difference between the least and most busy Agents.
        /// </summary>
        public int MaxPointDiscrepancy
        {
            get
            {
                return _agents.Max(agent => agent.TotalPrimaryPoints) -
                    _agents.Min(agent => agent.TotalPrimaryPoints);
            }
        }

        public decimal AverageSpread
        {
            get
            {
                int up = 0;
                int down = 0;
                foreach (var agent in _agents)
                {
                    var previousDay = new DateTime?();
                    foreach (var period in this)
                    {
                        if (period.Primary == agent)
                        {
                            if (previousDay.HasValue)
                            {
                                up += (period.Day - previousDay.Value).Days;
                                down += 1;
                            }
                            previousDay = period.Day;
                        }
                    }
                }

                return (decimal)up / down;
            }
        }

        public int MinimumSpread
        {
            get
            {
                var minimum = int.MaxValue;
                foreach (var agent in _agents)
                {
                    var previousDay = new DateTime?();
                    foreach (var period in this)
                    {
                        if (period.Primary == agent)
                        {
                            if (previousDay.HasValue)
                            {
                                var spread = (period.Day - previousDay.Value).Days;
                                if (spread < minimum)
                                {
                                    minimum = spread;
                                }
                            }
                            previousDay = period.Day;
                        }
                    }
                }

                return minimum;
            }
        }

        private readonly List<Agent> _agents;
        private readonly List<DateTime> _statDays = new List<DateTime>();

        public ReadOnlyCollection<DateTime> StatDays { get { return new ReadOnlyCollection<DateTime>(_statDays); } }

        public Schedule(IEnumerable<Agent> agents, DateTime start, DateTime end)
        {
            validateArguments(agents, start, end);

            createPeriods(start, end);
            _agents = agents.ToList();
        }

        public Schedule(IEnumerable<Agent> agents, DateTime start, int days)
        {
            var end = start.AddDays(days);
            validateArguments(agents, start, end);
            createPeriods(start, end);
            _agents = agents.ToList();
        }

        public Schedule(IEnumerable<Agent> agents, DateTime start, int days, IEnumerable<DateTime> StatDays)
            : this(agents, start, days)
        {
            _statDays = StatDays.ToList();
            foreach (var period in this.Join(_statDays,
                outerPeriod => new
                {
                    Year = outerPeriod.Day.Year,
                    Month = outerPeriod.Day.Month,
                    Day = outerPeriod.Day.Day
                },
                statDay => new
                {
                    Year = statDay.Year,
                    Month = statDay.Month,
                    Day = statDay.Day
                },
                    (period1, statDay) => period1))
            {
                period.IsStatDay = true;
            }
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
                if (start != i)
                {
                    _agents.Shuffle(start, i);
                    start = i;
                }
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

        private void assignAvailables()
        {
            foreach (var period in this)
            {
                period.AvailableAgents.Clear();
                foreach (var agent in _agents)
                {
                    if (agent.IsAvailable(period.Day))
                    {
                        period.AvailableAgents.Add(agent);
                    }
                }
            }
        }

        private const ScheduleFillingStrategy Strategy = ScheduleFillingStrategy.ByLeastAvailablesFirst;

        private void assignPrimaries(ScheduleFillingStrategy strategy)
        {
            switch (strategy)
            {
                case ScheduleFillingStrategy.Chronological:
                    assignPrimariesChronologically();
                    break;
                case ScheduleFillingStrategy.ByLeastAvailablesFirst:
                    assignPrimariesByAvailability();
                    break;
                default:
                    throw new Exception("Unknown strategy " + strategy);
            }
        }

        private Period previousPeriod(Period period)
        {
            if (this[0] == period)
            {
                return null;
            }
            var index = IndexOf(period);
            if (index == -1)
            {
                throw new Exception("Period wasn't from this schedule.");
            }

            return this[index - 1];
        }

        private Period nextPeriod(Period period)
        {
            if (this.Last() == period)
            {
                return null;
            }
            var index = IndexOf(period);
            if (index == -1)
            {
                throw new Exception("Period wasn't from this schedule.");
            }

            return this[index + 1];
        }

        private bool IsCandidateValid
            (Period period, Agent agent)
        {
            var previousDay = previousPeriod(period);
            var nextDay = nextPeriod(period);

            return agent.IsAvailable(period.Day)
                        && (previousDay == null || previousDay.Primary != agent) //Didn't work yesterday
                        && (nextDay == null || nextDay.Primary != agent) //Isn't working tomorrow
                        && (!period.Day.IsWeekEnd() ||
                            (!agentWorkedLastWeekEnd(agent, period)
                            && !agentIsWorkingNextWeekEnd(agent, period)));
        }

        /// <summary>
        /// Distance is the number of days between period and the last or next time an agent is assigned as Primary.
        /// </summary>
        /// <param name="period"></param>
        /// <param name="agent"></param>
        /// <returns></returns>
        private int GetDistance(Period period, Agent agent)
        {
            var distanceBefore = int.MaxValue;
            for (int i = IndexOf(period) - 1; i >= 0; i--)
            {
                if (this[i].Primary == agent)
                {
                    distanceBefore = IndexOf(period) - i;
                    break;
                }
            }
            var distanceAfter = int.MaxValue;
            for (int i = IndexOf(period) + 1; i < this.Count; i++)
            {
                if (this[i].Primary == agent)
                {
                    distanceAfter = i - IndexOf(period);
                    break;
                }
            }

            return Math.Min(distanceAfter, distanceBefore);
        }

        private void assignPrimariesByAvailability()
        {
            assignAvailables();
            foreach (var period in this.OrderBy(period => period.AvailableAgents.Count)
                .ThenByDescending(period => period.PointValue))
            {
                if (period.Primary != null)
                {
                    continue;
                }

                var candidates = period.AvailableAgents
                    .Where(candidate => IsCandidateValid(period, candidate));

                if (!candidates.Any())
                {
                    continue;
                }

                var minTotalPoints = candidates.Min(candidate => candidate.TotalPrimaryPoints);

                var bestCandidate = candidates.Where(candidate => candidate.TotalPrimaryPoints < minTotalPoints + 2)
                    .OrderBy(candidate => GetDistance(period, candidate)).LastOrDefault();
                AssignPrimary(period, bestCandidate);
            }
        }

        private void assignPrimariesChronologically()
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
                    if (IsCandidateValid(period, agent))
                    {
                        AssignPrimary(period, agent);
                        reorderAgent(agent);
                        break;
                    }
                }
            }
        }

        private void assignPrimaries()
        {
            assignPrimaries(Strategy);
        }

        private bool agentWorkedLastWeekEnd(Agent agent, Period period)
        {
            var lastWeekEnd = period.Day.GetLastWeekEnd();
            return agentWorked(agent, lastWeekEnd);
        }

        private bool agentIsWorkingNextWeekEnd(Agent agent, Period period)
        {
            var nextWeekEnd = period.Day.GetNextWeekEnd();
            return agentWorked(agent, nextWeekEnd);
        }

        private bool agentWorked(Agent agent, DateTime[] days)
        {
            foreach (var day in days)
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

        public void AssignPrimary
            (Period period, Agent agent)
        {
            period.Primary = agent;
            agent.PrimaryCount += 1;
            agent.PrimaryPoints += period.PointValue;
        }

        /// <summary>
        /// Moves provided agent after all other agent with equal TotalPrimaryPoints
        /// </summary>
        /// <param name="agent"></param>
        private void reorderAgent(Agent agent)
        {
            _agents.Remove(agent);
            for (int i = 0; i < _agents.Count; i++)
            {
                if (_agents[i].TotalPrimaryPoints > agent.TotalPrimaryPoints)
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
            sb.AppendLine("MaxDiscrepancy:" + MaxPointDiscrepancy +
                ", Average Spread: " + AverageSpread +
                ", Minimum Spread: " + MinimumSpread);
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
