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
    /// Interaction logic for InventoryLogs.xaml
    /// </summary>
    public partial class InventoryLogs : UserControl
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        //SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt = new DataTable();

        public InventoryLogs()
        {
            InitializeComponent();
            show();
        }

        private void show()
        {
            da = new SqlDataAdapter("Select pli.id, pi.id as productid, pc.name, pii.itemname, pli.activity, pli.change, pa.name as empname, pli.date, pi.isActive FROM posproject.category pc JOIN posproject.inventory_items pii ON pc.id = pii.category_id JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.log_inventory pli ON pi.id = pli.inventory_id JOIN posproject.accounts pa ON pli.account_id = pa.id", con);

            da.Fill(dt);
            logsDatagrid.ItemsSource = dt.DefaultView;
        }

        private void searchBar(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Format("itemname like '%{0}%'", searchTxt.Text);
                logsDatagrid.ItemsSource = dv;
            }
        }

        private void allAccounts(object sender, RoutedEventArgs e)
        {
            InitializeComponent();
            Content = new inventoryLogsModels();
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
            Content = new inventoryLogsModels();
        }
    }
}
