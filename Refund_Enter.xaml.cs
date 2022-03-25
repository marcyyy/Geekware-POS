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

namespace posproj
{
    public partial class Refund_Enter : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");


        public Refund_Enter()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void clearBtn(object sender, RoutedEventArgs e)
        {
            refundNo.Clear();
        }

        private void confirmBtn(object sender, RoutedEventArgs e)
        {
            string action = "Refund";

            con.Open();
            SqlCommand cmd1 = new SqlCommand("Select * from posproject.order_transaction where activity = @activity AND ref_num = @ref_num", con);
            cmd1.Parameters.AddWithValue("@activity", action);
            cmd1.Parameters.AddWithValue("@ref_num", refundNo.Text);

            SqlDataReader rd1 = cmd1.ExecuteReader();
            bool hasRows1 = (rd1.HasRows);
            con.Close();

            if (hasRows1 == false)
            {
                //check if valid transac num
                string refundNumber = "";

                con.Open();
                SqlCommand cmd0 = new SqlCommand("Select * From posproject.order_transaction Where ref_num=@ref_num", con);
                cmd0.Parameters.AddWithValue("@ref_num", refundNo.Text);

                SqlDataReader rd = cmd0.ExecuteReader();
                bool hasRows = (rd.HasRows);

                while (rd.Read())
                {
                    refundNumber = rd[1].ToString();
                }

                con.Close();

                if (hasRows == true)
                {
                    refundData.RefundAmount = 0;
                    refundData.RefundNum = refundNumber;
                    Refund_Main refund = new Refund_Main();
                    refund.Owner = this;
                    this.Hide();
                    refund.Show();
                }
                else
                {
                    MessageBox.Show("Transaction # does not exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Refund transaction already exists", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}

