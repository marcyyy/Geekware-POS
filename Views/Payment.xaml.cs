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

namespace posproj.Views
{
    public partial class Payment : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;


        string transacNum = generateCode.transacCode;

        public Payment()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            getData();
        }

        void getData()
        {
            string initAmnt = "";

            con.Open();
            SqlCommand cmd0 = new SqlCommand("Select SUM(amount) From posproject.order_items Where ref_num=@ref_num", con);
            cmd0.Parameters.AddWithValue("@ref_num", transacNum);

            SqlDataReader rd = cmd0.ExecuteReader();

            while (rd.Read())
            {
                initAmnt = rd.GetValue(0).ToString();
            }

            con.Close();

            totalTxt.Text = initAmnt;

        }

        public bool isValid()
        {
            if (paidTxt.Text == string.Empty)
            {
                MessageBox.Show("Enter Cash Paid", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (changeTxt.Text == string.Empty)
            {
                MessageBox.Show("Compute for change (=)", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            int totalPay = Int32.Parse(totalTxt.Text);
            int cashPay = Int32.Parse(paidTxt.Text);

            if (totalPay > cashPay)
            {
                MessageBox.Show("Amount Insufficient", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                changeTxt.Clear();  
                return false;
            }


            return true;
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void clearBtn(object sender, RoutedEventArgs e)
        {
            paidTxt.Clear();
        }

        private void confirmBtn(object sender, RoutedEventArgs e)
        {
            //insert into transactions
            if (isValid()) 
            {
                float initAmnt = float.Parse(totalTxt.Text);

                cmd = new SqlCommand("Insert into posproject.order_transaction(ref_num, order_date, total) Values (@ref_num, @order_date, @total)", con);
                cmd.Parameters.AddWithValue("@ref_num", transacNum);
                cmd.Parameters.AddWithValue("@order_date", DateTime.Now);
                cmd.Parameters.AddWithValue("@total", initAmnt);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();


                //insert into transactions logs

                String actLog = "Transaction Done";

                SqlCommand cmd1 = new SqlCommand("Insert into posproject.log_orders(date, ref_num, activity, account_id) Values (@date, @ref_num, @activity, @account_id)", con);
                cmd1.Parameters.AddWithValue("@date", DateTime.Now);
                cmd1.Parameters.AddWithValue("@ref_num", transacNum);
                cmd1.Parameters.AddWithValue("@activity", actLog);
                cmd1.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                con.Open();
                cmd1.ExecuteNonQuery();
                con.Close();

                Receipt receipt = new Receipt();
                receipt.Owner = this;
                this.Hide();
                receipt.Show();
            }

        }

        private void computeBtn(object sender, RoutedEventArgs e)
        {

            float payAmnt = float.Parse(paidTxt.Text);
            float initAmnt = float.Parse(totalTxt.Text);

            float totAmnt = payAmnt - initAmnt;
            string stringAmnt = totAmnt.ToString();

            changeTxt.Text = stringAmnt;

            receiptData.ProductCash = payAmnt;
            receiptData.ProductChange = totAmnt;
        }

        private void paidTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(paidTxt.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                paidTxt.Text = paidTxt.Text.Remove(paidTxt.Text.Length - 1);
            }
        }
    } 
}
