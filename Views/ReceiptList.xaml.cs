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
    /// Interaction logic for ReceiptList.xaml
    /// </summary>
    public partial class ReceiptList : UserControl
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;
        SqlDataAdapter da;

        string transacNum = generateCode.transacCode;
        string initAmnt = "";

        public ReceiptList()
        {
            InitializeComponent();
            queryItem();

            transNumTxt.Content = transacNum;

            computeTotal();
            payableTxt.Content = initAmnt;

            showReceipt();
            showInventory();

            var dateToday = DateTime.Now;
            var dateShow = dateToday.ToLongDateString();
            dateTodayTxt.Content = dateShow;
        }

        DataTable dt = new DataTable();
        DataTable dt2 = new DataTable();

        public void showReceipt()
        {
            da = new SqlDataAdapter("Select poi.id, poi.ref_num, pii.itemname, poi.quantity, poi.amount from posproject.inventory_items pii JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.order_items poi ON pi.id = poi.inventory_id WHERE poi.ref_num = '" + transacNum + "' ", con);

            da.Fill(dt);
            receiptDatagrid.ItemsSource = dt.DefaultView;
        }

        public void showInventory()
        {
            SqlDataAdapter da2 = new SqlDataAdapter("Select pi.id, pc.name, pii.itemname, pi.size, pi.color, pi.price from posproject.category pc JOIN posproject.inventory_items pii ON pc.id = pii.category_id JOIN posproject.inventory pi ON pii.id = pi.item_id WHERE pi.isActive='Yes' AND pi.quantity > 0 ORDER BY pc.name ", con);

            da2.Fill(dt2);
            inventoryDatagrid.ItemsSource = dt2.DefaultView;
        }

        private void computeTotal()
        {
            con.Open();
            SqlCommand cmd0 = new SqlCommand("Select SUM(amount) From posproject.order_items Where ref_num=@ref_num", con);
            cmd0.Parameters.AddWithValue("@ref_num", transacNum);

            SqlDataReader rd = cmd0.ExecuteReader();

            while (rd.Read())
            {
                initAmnt = rd.GetValue(0).ToString();
            }

            con.Close();
        }

        public void queryItem()
        {
            con.Open();
            cmd = new SqlCommand("Select pi.id, pc.name, pii.itemname, pi.location, pi.price  from posproject.category pc JOIN posproject.inventory_items pii ON pii.category_id = pc.id JOIN posproject.inventory pi ON pii.id = pi.item_id WHERE pi.isActive = 'Yes' AND pi.quantity > 0 ORDER BY pc.name", con);
            SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                int productId = (int)rd.GetValue(0);
                string productCategory = rd.GetValue(1).ToString();
                string productName = rd.GetValue(2).ToString();
                string productImage = rd.GetValue(3).ToString();
                string productPrice = rd.GetValue(4).ToString();

                generateItem(productId, productCategory, productName, productImage, productPrice);
            }
            con.Close();
        }

        private void generateItem(int productId, string productCategory, string productName, string productImage, string productPrice)
        {
            cardData.ProductId = productId;
            cardData.ProductCategory = productCategory;
            cardData.ProductName = productName;
            cardData.ProductImage = productImage;
            cardData.ProductPrice = productPrice;

            ItemCard ic = new ItemCard();
            stackItem.Children.Add(ic);
        }

        public void contentRefresh() 
        {
            InitializeComponent();
            Content = new listModels();
        }

        private void cancelBtn(object sender, RoutedEventArgs e)
        {
            /*
            con.Open();
            SqlCommand cmd0 = new SqlCommand("Select * From posproject.order_items Where ref_num=@ref_num", con);
            cmd0.Parameters.AddWithValue("@ref_num", transacNum);

            SqlDataReader rd = cmd0.ExecuteReader();
            bool hasRows = (rd.HasRows);

            con.Close();

            if (hasRows == true)
            {
                con.Open();
                SqlCommand cmd2 = new SqlCommand("Select inventory_id, quantity from posproject.order_items Where ref_num=@ref_num", con);
                cmd2.Parameters.AddWithValue("@ref_num", transacNum);
                SqlDataReader rd1 = cmd2.ExecuteReader();

                while (rd1.Read())
                {
                    cancelOrder.InventoryId = (int)rd1.GetValue(0);
                    cancelOrder.Quantity = (int)rd1.GetValue(1);
                }
                con.Close();
            }
            else
            {
                MessageBox.Show("Select Items first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }*/

            User_Register ur = new User_Register();
            ur.Show();
        }

        private void payBtn(object sender, RoutedEventArgs e)
        {
            con.Open();
            SqlCommand cmd0 = new SqlCommand("Select * From posproject.order_items Where ref_num=@ref_num", con);
            cmd0.Parameters.AddWithValue("@ref_num", transacNum);

            SqlDataReader rd = cmd0.ExecuteReader();
            bool hasRows = (rd.HasRows);

            con.Close();

            if (hasRows == true)
            {
                Payment payCtrl = new Payment();
                payCtrl.Show();
            }
            else
            {
                MessageBox.Show("Select Items first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
