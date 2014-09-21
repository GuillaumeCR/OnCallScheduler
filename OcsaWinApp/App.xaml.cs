using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using OnCallScheduler;
using Newtonsoft.Json;
using System.IO;
using System.Collections.ObjectModel;

namespace OcsaWinApp
{
    public interface IOcsaWinApp
    {
        Schedule Schedule { get; }
        ObservableCollection<Agent> Agents { get; }
        void SaveAgents();

        void ClearAgentPoints();
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IOcsaWinApp
    {
        private const string AgentFileName = "Agents.txt";

        void App_Startup(object sender, StartupEventArgs e)
        {
            loadAgentFile();
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        void App_Exit(object sender, ExitEventArgs e)
        {
            SaveAgents();
        }

        private const string AgentFile = "Agents.txt";

        private void loadAgentFile()
        {
            if (File.Exists(AgentFile))
            {
                Agents = JsonConvert.DeserializeObject<ObservableCollection<Agent>>(File.ReadAllText(AgentFile));
                ClearAgentPoints();
            }
            else
            {
                Agents = new ObservableCollection<Agent>();
            }

            
        }

        public void SaveAgents()
        {
            var content = JsonConvert.SerializeObject(Agents);
            File.WriteAllText(AgentFile, content);
        }

        public Schedule Schedule
        {
            get { return (Schedule)Properties["Schedule"]; }
            set { Properties["Schedule"] = value; }
        }

        public ObservableCollection<Agent> Agents
        {
            get { return (ObservableCollection<Agent>)Properties["Agents"]; }
            set { Properties["Agents"] = value; }
        }


        public void ClearAgentPoints()
        {
            //TODO: Probably need an AgentList that takes care of this.
            foreach (var agent in Agents)
            {
                agent.PrimaryCount = 0;
                agent.PrimaryPoints = 0;
            }
        }
    }
}
