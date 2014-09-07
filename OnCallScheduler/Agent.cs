using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace OnCallScheduler
{
    public class Agent
    {
        public string Name { get; set; }
        public int PrimaryCount { get; set; }
        public int BackupCount { get; set; }
        public int PointReduction { get; set; }
        [JsonIgnore]
        public int PrimaryPoints { get { return PrimaryCount + PointReduction; } }

        private List<DateTime> _cantWorkOn;
        public List<DateTime> CantWorkOn
        {
            get { return _cantWorkOn; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }
                _cantWorkOn = value;
            }
        }

        public Agent()
        {
            _cantWorkOn = new List<DateTime>();
        }

        public override string ToString()
        {
            return Name;
        }

        public bool IsAvailable(DateTime period)
        {
            foreach (var vacation in CantWorkOn)
            {
                if (vacation.Date == period.Date)
                {
                    return false;
                }
            }
            return true;
        }

        public class LeastPrimary : IComparer<Agent>
        {
            public int Compare(Agent first, Agent second)
            {
                if (first == null || second == null)
                {
                    throw new ArgumentNullException();
                }
                return first.PrimaryPoints - second.PrimaryPoints;
            }
        }

        public class LeastBackup : IComparer<Agent>
        {
            public int Compare(Agent first, Agent second)
            {
                if (first == null || second == null)
                {
                    throw new ArgumentNullException();
                }
                return first.BackupCount - second.BackupCount;
            }
        }
    }
}
