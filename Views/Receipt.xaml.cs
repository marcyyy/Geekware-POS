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
using System.Windows.Shapes;
using System.IO;
using System.Windows.Xps.Packaging;
using System.Data.SqlClient;

namespace posproj.Views
{
    public partial class Receipt : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;

        public Receipt()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            queryItem();

            var dateToday = DateTime.Now.ToString();
            dateTxt.Text = dateToday;

            transacNumTxt.Text = generateCode.transacCode;
            userTxt.Text = loginInfo.accountName;

            totalTxt.Text = "₱" + receiptData.Total.ToString();
            cashTxt.Text = "₱" + receiptData.ProductCash.ToString();
            changeTxt.Text = "₱" + receiptData.ProductChange.ToString();

        }

        public void CloseAllWindows()
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
                App.Current.Windows[intCounter].Hide();
        }

        private void closeBtn(object sender, RoutedEventArgs e)
        {
            CloseAllWindows();
            User_Register userRegister = new User_Register();
            userRegister.Show();
        }

        private void printBtn(object sender, RoutedEventArgs e)
        {
            
            PrintDialog printDlg = new PrintDialog();
            if (printDlg.ShowDialog() == true)
            printDlg.PrintVisual(receiptGrid, "Receipt");
        }

        public void queryItem()
        {
            con.Open();
            cmd = new SqlCommand("Select pii.itemname, pi.price, poi.quantity, poi.amount, pot.total from posproject.inventory_items pii JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.order_items poi ON pi.id = poi.inventory_id JOIN posproject.order_transaction pot ON poi.ref_num = pot.ref_num WHERE pot.ref_num=@ref_num", con);
            cmd.Parameters.AddWithValue("@ref_num", generateCode.transacCode);
            SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                string productName = rd.GetValue(0).ToString();
                float productPrice = Convert.ToSingle(rd.GetValue(1));
                int productQuantity = (int)rd.GetValue(2);
                float productAmount = Convert.ToSingle(rd.GetValue(3));
                int productTotal = (int)rd.GetValue(4);

                generateItem(productName, productPrice, productQuantity, productAmount, productTotal);
            }
            con.Close();
        }

        private void generateItem(string productName, float productPrice, int productQuantity, float productAmount, int productTotal)
        {
            receiptData.ProductName = productName;
            receiptData.ProductPrice = productPrice;
            receiptData.ProductQuantity = productQuantity;
            receiptData.ProductAmount = productAmount;
            receiptData.Total = productTotal;

            ReceiptData rd = new ReceiptData();
            stackItem.Children.Add(rd);
        }
    }
}
