using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Xml;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Tasks.v1.Data;
using Google.Apis.Util.Store;

namespace TaskBatcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            // This initializes our window
            InitializeComponent();

            // Setup the connection to the Google Tasks API
            SetupService();

            // Get this week's Monday and the following 2
            List<DateTime> mondays = GetMondays();

            // Add first Monday to ListBox
            listBoxWeeks.Items.Insert(0, mondays[0].Date.ToString("d"));
            // Add second Monday to ListBox
            listBoxWeeks.Items.Insert(1, mondays[1].Date.ToString("d"));
            // Add third Monday to ListBox
            listBoxWeeks.Items.Insert(2, mondays[2].Date.ToString("d"));
        }

        // When the add button is clicked..
        private void button_Click(object sender, RoutedEventArgs e)
        {
            // Grab the task
            string newTask = taskToAdd.Text;

            // Get all of the checked days
            List<string> selectedDays = GetDays();

            // Get all of the selected weeks
            List<string> selectedWeeks = GetWeeks();

            // Get this monday and the next 2
            List<DateTime> mondays = GetMondays();

            // Check if any of our values are empty/null
            if (isAnythingEmpty(newTask, selectedDays, selectedWeeks))
                return;

            // Get the exact days to add to
            List<DateTime> exactDays = GetExactDays(mondays, selectedDays, selectedWeeks);

            // Grab the default task list
            TaskList defaultList = Service.Tasklists.Get("@default").Execute();
            // Grab a list of all tasks in that tasklist
            Tasks allTasks = Service.Tasks.List("@default").Execute();
            StringBuilder status = new StringBuilder();
            status.Append("Successfully Added: \n");

            foreach(DateTime day in exactDays)
            {
                // Make a new Google Task
                Task task = new Task();

                // Give the new task the title we entered
                task.Title = newTask;

                // Give the task a due date
                task.Due = day;

                // Get the result
                Task result = Service.Tasks.Insert(task, "@default").Execute();

                status.Append(result.Title + "\n");
            }
            MessageBox.Show(status.ToString()); 
        }

        // This sets up the connection with Google Tasks
        public async void SetupService()
        {
            // Create the service.
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                                    GoogleClientSecrets.Load(stream).Secrets,
                                    new[] { TasksService.Scope.Tasks },
                                    "user", CancellationToken.None, new FileDataStore("Tasks.Auth.Store"));

                Service = new TasksService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "TaskBatcher",
                });
            }
        }

        // This gets a list of all checkboxes we chose
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
        
        // This returns a list of this week's monday and the next 2
        private List<DateTime> GetMondays()
        {
            // Get the Monday of the current week
            DateTime thisMonday = GetCurrentMonday();
            // Get the Monday of the next week
            DateTime nextMonday = thisMonday.AddDays(7);
            // Get the Monday of the 3rd week out
            DateTime thirdMonday = nextMonday.AddDays(7);

            // Create a temporary list
            List<DateTime> mondays = new List<DateTime>();

            // Add our Mondays to the list
            mondays.Add(thisMonday);
            mondays.Add(nextMonday);
            mondays.Add(thirdMonday);

            // Return the list
            return mondays;
        }

        // This gets the Monday of the week we're on (even if passed)
        private DateTime GetCurrentMonday()
        {
            // Get today's date
            DateTime today = DateTime.Today;

            // Create a blank DateTime object
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
            // Give back this Monday
            return thisMonday;
        }
        
        // This gives us back the weeks we've selected to add to
        private List<string> GetWeeks()
        {
            // Create an empty list of strings
            List<string> selectedWeeks = new List<string>();
                
            // For each week we've selected
            foreach(var item in listBoxWeeks.SelectedItems)
            {
                // Add them to our list
                selectedWeeks.Add(item.ToString());
            }

            // Hand back the list
            return selectedWeeks;
        }

        // This checks to make sure we have filled everything out in the UI
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

        // This gets the exact days we want to add to, as strings in RFC3339 format
        private List<DateTime> GetExactDays(List<DateTime> mondays, List<string> selectedDays, List<string> selectedWeeks)
        {
            List<DateTime> exactDays = new List<DateTime>();
            DateTime specificMonday = new DateTime();

            // Loop through our list
            for(int i=0; i < selectedWeeks.Count; i++)
            {
                // Figure out which Monday we have
                if(selectedWeeks[i] == mondays[0].ToString("d"))
                {
                    specificMonday = mondays[0];
                }
                else if(selectedWeeks[i] == mondays[1].ToString("d"))
                {
                    specificMonday = mondays[1];
                }
                else
                {
                    specificMonday = mondays[2];
                }

                // Get the exact day in DateTime
                foreach(string day in selectedDays)
                {
                    switch(day)
                    {
                        case "Monday":
                            exactDays.Add(specificMonday);
                            break;

                        case "Tuesday":
                            exactDays.Add(specificMonday.AddDays(1));
                            break;

                        case "Wednesday":
                            exactDays.Add(specificMonday.AddDays(2));
                            break;

                        case "Thursday":
                            exactDays.Add(specificMonday.AddDays(3));
                            break;

                        case "Friday":
                            exactDays.Add(specificMonday.AddDays(4));
                            break;

                        case "Saturday":
                            exactDays.Add(specificMonday.AddDays(5));
                            break;

                        case "Sunday":
                            exactDays.Add(specificMonday.AddDays(6));
                            break;
                    }
                }
            }

            // Return the list
            return exactDays;
        }

        public static TasksService Service { get; private set; }
    }
}
