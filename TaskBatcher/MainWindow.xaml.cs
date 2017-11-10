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

namespace TaskBatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            List<DateTime> mondays = GetMondays();


            // Add first Monday to ListBox
            listBoxWeeks.Items.Insert(0, mondays[0].Date.ToString("d"));
            // Add second Monday to ListBox
            listBoxWeeks.Items.Insert(1, mondays[1].Date.ToString("d"));
            // Add third Monday to ListBox
            listBoxWeeks.Items.Insert(2, mondays[2].Date.ToString("d"));
        }

  

        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Grab the task
            string newTask = taskToAdd.Text;

            // Get all of the checked days
            List<string> selectedDays = GetDays();

            // Get all of the selected weeks
            List<string> selectedWeeks = GetWeeks();

            // Check if any of our values are empty/null
            if (isAnythingEmpty(newTask, selectedDays, selectedWeeks))
                return;
        }

        private List<string> GetDays()
        {
            // Make a temporary list
            List<string> selectedDays = new List<string>();

            // For each checkbox item
            foreach (CheckBox box in layoutGrid.Children.OfType<CheckBox>()) 
            {
                // IF the box is checked, add its value to our list
                if(box.IsChecked == true)
                {
                    selectedDays.Add((string)box.Content);
                }
            }

            // Hand off the list
            return selectedDays;
        }
        private List<DateTime> GetMondays()
        {
            // Get the Monday of the current week
            DateTime thisMonday = GetCurrentMonday();
            // Get the Monday of the next week
            DateTime nextMonday = thisMonday.AddDays(7);
            // Get the Monday of the 3rd week out
            DateTime thirdMonday = nextMonday.AddDays(7);

            List<DateTime> mondays = new List<DateTime>();

            mondays.Add(thisMonday);
            mondays.Add(nextMonday);
            mondays.Add(thirdMonday);

            return mondays;
        }
        private DateTime GetCurrentMonday()
        {
            // Get today's date
            DateTime today = DateTime.Today;

            DateTime thisMonday = new DateTime();

            // If today's date is...
            switch (today.DayOfWeek.ToString())
            {
                // If it's Monday, save that!
                case "Monday":
                    thisMonday = today;
                    break;

                // If it's Tuesday, subtract a day to find Monday
                case "Tuesday":
                    thisMonday = today.AddDays(-1);
                    break;

                // If it's Wednesday, subtract two days to find Monday
                case "Wednesday":
                    thisMonday = today.AddDays(-2);
                    break;

                // If it's Thursday, subtract three days to find Monday
                case "Thursday":
                    thisMonday = today.AddDays(-3);
                    break;

                // If it's Friday, subtract four days to find Monday
                case "Friday":
                    thisMonday = today.AddDays(-4);
                    break;

                // If it's Saturday, subtract five days to find Monday
                case "Saturday":
                    thisMonday = today.AddDays(-5);
                    break;

                // If it's Sunday, subtract six days to find Monday
                case "Sunday":
                    thisMonday = today.AddDays(-6);
                    break;
            }
            return thisMonday;
        }
        private List<string> GetWeeks()
        {
            List<string> selectedWeeks = new List<string>();
                
            foreach(var item in listBoxWeeks.SelectedItems)
            {
                selectedWeeks.Add(item.ToString());
            }

            return selectedWeeks;
        }
        private Boolean isAnythingEmpty(string newTask, List<string> selectedDays, List<string> selectedWeeks)
        {
            Boolean empty = false;

            // If our task is empty, go back
            if (newTask == "")
            {
                MessageBox.Show("You must enter a task to add!");
                empty = true;
            }
            
            // If no days were checked, go back
            if (!selectedDays.Any())
            {
                MessageBox.Show("You must pick days to add your task to!");
                empty = true;
            }

            // If no weeks were selected, go back
            if (!selectedWeeks.Any())
            {
                MessageBox.Show("You must pick at least one week!");
                empty = true;
            }

            return empty;
        }
    }
}
