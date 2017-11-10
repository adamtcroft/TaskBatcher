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
            DateTime thisMonday = GetCurrentMonday();
            DateTime nextMonday = thisMonday.AddDays(7);
            DateTime thirdMonday = nextMonday.AddDays(7);

            listBoxWeeks.Items.Insert(0, thisMonday.Date.ToString("d"));
            listBoxWeeks.Items.Insert(1, nextMonday.Date.ToString("d"));
            listBoxWeeks.Items.Insert(2, thirdMonday.Date.ToString("d"));
        }

        public DateTime GetCurrentMonday()
        {
            DateTime today = DateTime.Today;
            DateTime thisMonday = new DateTime();

            switch(today.DayOfWeek.ToString())
            {
                case "Monday":
                    thisMonday = today;
                    break;

                case "Tuesday":
                    thisMonday = today.AddDays(-1);
                    break;

                case "Wednesday":
                    thisMonday = today.AddDays(-2);
                    break;

                case "Thursday":
                    thisMonday = today.AddDays(-3);
                    break;

                case "Friday":
                    thisMonday = today.AddDays(-4);
                    break;

                case "Saturday":
                    thisMonday = today.AddDays(-5);
                    break;

                case "Sunday":
                    thisMonday = today.AddDays(-6);
                    break;
            }
            return thisMonday;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string newTask = taskToAdd.Text;


        }
    }
}
