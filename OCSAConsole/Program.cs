using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnCallScheduler;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace OCSAConsole
{
    class Program
    {
        public const string DefaultAgentFile = "Agents.txt";
        static void Main(string[] args)
        {
            try
            {
                ParseArgs(args);
                if (!File.Exists(_agentFile))
                {
                    throw new FileNotFoundException("Could not load file " + _agentFile);
                }

                var agents = JsonConvert.DeserializeObject<List<Agent>>(File.ReadAllText(DefaultAgentFile));
                var schedule = new Schedule(agents, _start, _start + new TimeSpan(28, 0, 0, 0));
                File.WriteAllText("Schedule.txt", schedule.ToString());
                Process.Start("Schedule.txt");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static string _agentFile;
        private static DateTime _start;

        private static void ParseArgs(string[] args)
        {
            if (args.Count() > 2)
            {
                throw new ArgumentException("Usage: ocsa [AgentFile] [StartDate]");
            }

            if (args.Count() == 2)
            {
                _agentFile = args[0];
                if (!DateTime.TryParse(args[1], out _start))
                {
                    throw new ArgumentException("Could not parse date " + args[1]);
                }
            }
            else if (args.Count() == 1)
            {
                if (DateTime.TryParse(args[0], out _start))
                {
                    _agentFile = DefaultAgentFile;
                }
                else
                {
                    _agentFile = args[0];
                    _start = DateTime.Now;
                }
            }
            else
            {
                _agentFile = DefaultAgentFile;
                _start = DateTime.Now;
            }
        }
    }
}
