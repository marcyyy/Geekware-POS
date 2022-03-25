using posproj.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace posproj
{
    /// <summary>
    /// Interaction logic for Admin_Homepage.xaml
    /// </summary>
    public partial class Admin_Homepage : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");

        public Admin_Homepage()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            Admin_HomepageLoad();

            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();

        }

        public void Admin_HomepageLoad()
        {   
            userID.Content = loginInfo.accountName;
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
        }

        private void logoutBtn(object sender, RoutedEventArgs e)
        {

            string activityRecord = "Logged out";

            SqlCommand cmd1 = new SqlCommand("Insert into posproject.log_accounts(account_id,date_time,activity) Values (@account_id,@date_time,@activity)", con);
            cmd1.Parameters.AddWithValue("@account_id", loginInfo.accountID);
            cmd1.Parameters.AddWithValue("@date_time", DateTime.Now);
            cmd1.Parameters.AddWithValue("@activity", activityRecord);

            con.Open();
            cmd1.ExecuteNonQuery();
            con.Close();

            MainWindow logout = new MainWindow();
            logout.Owner = this;
            this.Hide();
            logout.Show();
        }

        private void inventoryBtn(object sender, RoutedEventArgs e)
        {
            Admin_Inventory inventory = new Admin_Inventory();
            inventory.Owner = this;
            this.Hide();
            inventory.Show();
        }

        private void logsBtn(object sender, RoutedEventArgs e)
        {
            Admin_Logs logs = new Admin_Logs();
            logs.Owner = this;
            this.Hide();
            logs.Show();
        }

        private void reportBtn(object sender, RoutedEventArgs e)
        {
            Admin_Reports reports = new Admin_Reports();
            reports.Owner = this;
            this.Hide();
            reports.Show();
        }

        private void accountBtn(object sender, RoutedEventArgs e)
        {
            Admin_Accounts accounts = new Admin_Accounts();
            accounts.Owner = this;
            this.Hide();
            accounts.Show();
        }
    }
}
