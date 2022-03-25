using posproj.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace posproj.Views
{
    /// <summary>
    /// Interaction logic for AccountLogs.xaml
    /// </summary>
    public partial class AccountLogs : UserControl
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt = new DataTable();

        public AccountLogs()
        {
            InitializeComponent();
            show();
        }

        private void show()
        {
            da = new SqlDataAdapter("Select pla.id, pa.name, pla.activity, pla.date_time, pa.isActive FROM posproject.accounts pa JOIN posproject.log_accounts pla ON pa.id = pla.account_id", con);

            da.Fill(dt);
            logsDatagrid.ItemsSource = dt.DefaultView;
        }

        private void searchBar(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Format("name like '%{0}%'", searchTxt.Text);
                logsDatagrid.ItemsSource = dv;
            }
        }

        private void allAccounts(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            Content = new accountLogsModels();
        }

        private void userAccounts(object sender, RoutedEventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = string.Format("isActive like 'Yes'", searchTxt.Text);
            logsDatagrid.ItemsSource = dv;
        }

        private void adminAccounts(object sender, RoutedEventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = string.Format("isActive like 'No'", searchTxt.Text);
            logsDatagrid.ItemsSource = dv;
        }

        private void refreshBtn(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            Content = new accountLogsModels();
        }
    }
}
