using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OnCallScheduler;
using System.Collections.ObjectModel;
using System.IO;

using System.Diagnostics;

namespace OcsaWinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = Agents;

            AgentComboBox.SelectedIndex = 0;
        }

        public ObservableCollection<Agent> Agents { get { return ((IOcsaWinApp)App.Current).Agents; } }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var agent = new Agent { Name = NewAgentTextBox.Text };
            Agents.Add(agent);
            saveAgents();
        }

        private void DeleteAgentButton_Click(object sender, RoutedEventArgs e)
        {
            var agent = (Agent)AgentComboBox.SelectedItem;
            Agents.Remove(agent);
            saveAgents();
        }

        private void saveAgents()
        {
            ((IOcsaWinApp)App.Current).SaveAgents();
        }

        private void PointReductionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            saveAgents();
        }

        private void AgentComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bindCantWorkOnCalender();
        }

        private bool bindingCantWorkOn = false;

        private void bindCantWorkOnCalender()
        {
            bindingCantWorkOn = true;
            try
            {
                CantWorkOnCalender.SelectedDates.Clear();

                if (AgentComboBox.SelectedItem == null)
                {
                    return;
                }

                CurrentAgent.CantWorkOn.ForEach(date => CantWorkOnCalender.SelectedDates.Add(date));
            }
            finally
            {
                bindingCantWorkOn = false;
            }
        }

        private Agent CurrentAgent
        {
            get { return (Agent)AgentComboBox.SelectedItem; }
        }

        private void CantWorkOnCalender_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bindingCantWorkOn)
            {
                return;
            }

            CurrentAgent.CantWorkOn.Clear();
            foreach (var date in CantWorkOnCalender.SelectedDates)
            {
                CurrentAgent.CantWorkOn.Add(date);
            }
        }

        private void GenerateScheduleButton_Click(object sender, RoutedEventArgs e)
        {
            var statDays = StatDaysCalendar.SelectedDates.ToList<DateTime>();
            var schedule = new Schedule(Agents, ScheduleStartDatePicker.SelectedDate.Value , 28, statDays);
            schedule.FillUp();
            var filename = System.IO.Path.GetRandomFileName() + ".txt";
            File.WriteAllText(filename, schedule.ToString());
            Process.Start(filename);
            ((IOcsaWinApp)App.Current).ClearAgentPoints();
        }
    }
}
