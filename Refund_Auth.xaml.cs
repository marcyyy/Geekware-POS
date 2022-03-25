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
    public partial class Refund_Auth : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");

        public Refund_Auth()
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
            accName.Clear();
            accPass.Clear();
        }

        private void confirmBtn(object sender, RoutedEventArgs e)
        {
            SqlCommand cmd = new SqlCommand("role_login", con);
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            cmd.Parameters.AddWithValue("@vname", accName.Text);
            cmd.Parameters.AddWithValue("@vpassword", accPass.Password);

            SqlDataReader rd = cmd.ExecuteReader();

            if (rd.HasRows)
            {
                rd.Read();

                if (rd[3].ToString() == "Admin")
                {
                    Refund_Enter refundEnter = new Refund_Enter();
                    refundEnter.Owner = this;
                    this.Hide();
                    refundEnter.Show();
                }
                else if (rd[3].ToString() == "User")
                {
                    MessageBox.Show("Access Denied", "Error",  MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            else
            {
                MessageBox.Show("Account doesn't exist", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            con.Close();
        }

    }
}
