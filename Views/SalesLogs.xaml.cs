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
    public partial class SalesLogs : UserControl
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        //SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt = new DataTable();

        public SalesLogs()
        {
            InitializeComponent();
            show();
        }

        private void show()
        {
            da = new SqlDataAdapter("Select po.id, po.ref_num, po.activity, pa.name, po.date FROM posproject.accounts pa JOIN posproject.log_orders po ON pa.id = po.account_id", con);

            da.Fill(dt);
            logsDatagrid.ItemsSource = dt.DefaultView;
        }

        private void searchBar(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Format("ref_num like '%{0}%'", searchTxt.Text);
                logsDatagrid.ItemsSource = dv;
            }
        }

        private void adminAccounts(object sender, RoutedEventArgs e)
        {
            
        }

        private void refreshBtn(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            Content = new salesLogsModels();
        }

        private void logsDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {


        }
    }
}
