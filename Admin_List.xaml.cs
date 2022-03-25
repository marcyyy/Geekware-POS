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
using System.Windows.Shapes;

namespace posproj
{
    /// <summary>
    /// Interaction logic for Admin_List.xaml
    /// </summary>
    public partial class Admin_List : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlDataAdapter da;

        DataTable dt = new DataTable();
        string action = passUpdate.Action;
        string refNum = passUpdate.RefNum;

        public Admin_List()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            show();

            actionTxt.Text = action;
            refNumTxt.Text = refNum;
        }

        private void show()
        {
            string refund = "Sell";
            string refund2 = "Sell (Updated)";
            string refund3 = "Refund";

            if (action == refund)
            {
                da = new SqlDataAdapter("Select poi.id, poi.ref_num, pii.itemname, poi.quantity, poi.amount FROM posproject.inventory_items pii JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.order_items poi ON pi.id = poi.inventory_id JOIN posproject.order_transaction pot ON poi.ref_num = pot.ref_num  Where poi.ref_num = '"+ refNum + "' AND pot.activity = '" + action + "' ", con);
               
                da.Fill(dt);
                logsDatagrid.ItemsSource = dt.DefaultView;
            }

            else if (action == refund2)
            {
                da = new SqlDataAdapter("Select poi.id, poi.ref_num, pii.itemname, poi.quantity, poi.amount FROM posproject.inventory_items pii JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.order_items poi ON pi.id = poi.inventory_id JOIN posproject.order_transaction pot ON poi.ref_num = pot.ref_num  Where poi.ref_num = '" + refNum + "' AND pot.activity = '" + action + "' ", con);

                da.Fill(dt);
                logsDatagrid.ItemsSource = dt.DefaultView;
            }

            else if (action == refund3)
            {
                da = new SqlDataAdapter("Select poi.id, poi.ref_num, pii.itemname, poi.quantity, poi.amount FROM posproject.inventory_items pii JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.order_refund poi ON pi.id = poi.inventory_id JOIN posproject.order_transaction pot ON poi.ref_num = pot.ref_num  Where poi.ref_num = '" + refNum + "' AND pot.activity = '" + action + "' ", con);

                da.Fill(dt);
                logsDatagrid.ItemsSource = dt.DefaultView;
            }

        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
