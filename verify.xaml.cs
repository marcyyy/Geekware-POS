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
using System.Windows.Controls.Primitives;
using System.Data.SqlClient;
using System.Data;

namespace posproj
{
    public partial class verify : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;
        //SqlDataAdapter da;

        public verify()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            questionItems();
        }

        void questionItems()
        {

            con.Open();
            cmd = new SqlCommand("Select * from posproject.question ", con);
            SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                string questionName = rd.GetString(1);
                questionCombobox.Items.Add(questionName);
            }

            con.Close();

        }

        String questionId = "";

        public void questionConvert()
        {
            con.Open();

            SqlCommand selected = new SqlCommand("Select id from posproject.question Where question=@question", con);
            selected.Parameters.AddWithValue("@question", questionCombobox.Text);
            SqlDataReader rd = selected.ExecuteReader();


            while (rd.Read())
            {
                questionId = rd[0].ToString();
            }

            con.Close();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Submit(object sender, RoutedEventArgs e)
        {
            string accID = "";
            questionConvert();

            con.Open();
            SqlCommand cmd0 = new SqlCommand("Select id from posproject.accounts Where name=@name AND question=@question AND answer=@answer AND isActive= 'Yes' ", con);
            cmd0.Parameters.AddWithValue("@name", nameTxt.Text);
            cmd0.Parameters.AddWithValue("@question", questionId);
            cmd0.Parameters.AddWithValue("@answer", answerTxt.Text);

            SqlDataReader rd = cmd0.ExecuteReader();
            bool hasRows = (rd.HasRows);

            while (rd.Read())
            {
                accID = rd.GetValue(0).ToString();
            }
            con.Close();

            if (hasRows == true)
            {
                passUpdate.passwordID = accID;
                changePass passChange = new changePass();
                passChange.Owner = this;
                this.Hide();
                passChange.Show();
            }
            else 
            {
                MessageBox.Show("Account doesn't exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
             MainWindow login = new MainWindow();
            login.Owner = this;
            this.Hide();
            login.Show();
        }
    }
}
