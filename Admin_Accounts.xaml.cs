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
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using System.Data.SqlClient;
using System.Data;

namespace posproj
{
    public partial class Admin_Accounts : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd ;
        SqlDataAdapter da;

        public Admin_Accounts()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            show();
            questionItems();

            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
        }

        DataTable dt = new DataTable();

        private void show()
        {
            da = new SqlDataAdapter("Select pq.question, pa.id, pa.name, pa.password, pa.role, pa.answer from posproject.question pq INNER JOIN posproject.accounts pa ON pq.id = pa.question WHERE pa.isActive = 'Yes' ", con);

            da.Fill(dt);
            accountsDatagrid.ItemsSource = dt.DefaultView;


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

        private void insertBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    String selectedQuestion = questionCombobox.Text;
                    String questionId = "";

                    con.Open();

                    SqlCommand selected = new SqlCommand("Select id from posproject.question Where question=@name", con);
                    selected.Parameters.AddWithValue("@name", questionCombobox.Text);
                    SqlDataReader rd = selected.ExecuteReader();


                    while (rd.Read())
                    {
                        questionId = rd[0].ToString();
                    }

                    con.Close();

                    cmd = new SqlCommand("Insert into posproject.accounts(name,password,role,question,answer) Values (@name,@password,@role,@question,@answer)", con);
                    cmd.Parameters.AddWithValue("@name", nameTxt.Text);
                    cmd.Parameters.AddWithValue("@password", passTxt.Text);
                    cmd.Parameters.AddWithValue("@role", roleCombobox.Text);
                    cmd.Parameters.AddWithValue("@question", questionId);
                    cmd.Parameters.AddWithValue("@answer", answerTxt.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    //insert logs
                    string activityRecord = "Account Created";

                    SqlCommand cmd1 = new SqlCommand("Insert into posproject.log_accounts(account_id,date_time,activity) Values (@account_id,@date_time,@activity)", con);
                    cmd1.Parameters.AddWithValue("@account_id", loginInfo.accountID);
                    cmd1.Parameters.AddWithValue("@date_time", DateTime.Now);
                    cmd1.Parameters.AddWithValue("@activity", activityRecord);

                    con.Open();
                    cmd1.ExecuteNonQuery();
                    con.Close();

                    MessageBox.Show("Account Saved Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    show();
                    clearData();

                    Admin_Accounts refresh = new Admin_Accounts();
                    refresh.Owner = this;
                    this.Hide();
                    refresh.Show();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

        }

        private void updateBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {

                    String selectedQuestion = questionCombobox.Text;
                    String questionId = "";

                    con.Open();

                    SqlCommand selected = new SqlCommand("Select id from posproject.question Where question=@name", con);
                    selected.Parameters.AddWithValue("@name", questionCombobox.Text);
                    SqlDataReader rd = selected.ExecuteReader();


                    while (rd.Read())
                    {
                        questionId = rd[0].ToString();
                    }

                    con.Close();

                    cmd = new SqlCommand("Update posproject.accounts Set name=@name, password=@password, role= @role, question=@question, answer=@answer Where id=@id", con);
                    cmd.Parameters.AddWithValue("@id", idTxt.Text);
                    cmd.Parameters.AddWithValue("@name", nameTxt.Text);
                    cmd.Parameters.AddWithValue("@password", passTxt.Text);
                    cmd.Parameters.AddWithValue("@role", roleCombobox.Text);
                    cmd.Parameters.AddWithValue("@question", questionId);
                    cmd.Parameters.AddWithValue("@answer", answerTxt.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();


                    //insert logs
                    string activityRecord = "Account Updated";

                    SqlCommand cmd2 = new SqlCommand("Insert into posproject.log_accounts(account_id,date_time,activity) Values (@account_id,@date_time,@activity)", con);
                    cmd2.Parameters.AddWithValue("@account_id", idTxt.Text);
                    cmd2.Parameters.AddWithValue("@date_time", DateTime.Now);
                    cmd2.Parameters.AddWithValue("@activity", activityRecord);

                    con.Open();
                    cmd2.ExecuteNonQuery();
                    con.Close();

                    clearData();
                    MessageBox.Show("Account Updated Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    windowRefresh();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

        }
        
        private void deleteBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    String isActive = "No";

                    cmd = new SqlCommand("Update posproject.accounts Set isActive=@isActive Where id=@id", con);
                    cmd.Parameters.AddWithValue("@id", idTxt.Text);
                    cmd.Parameters.AddWithValue("@isActive", isActive);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    //insert logs 
                    string activityRecord = "Account Removed";

                    SqlCommand cmd3 = new SqlCommand("Insert into posproject.log_accounts(account_id,date_time,activity) Values (@account_id,@date_time,@activity)", con);
                    cmd3.Parameters.AddWithValue("@account_id", Int32.Parse(idTxt.Text));
                    cmd3.Parameters.AddWithValue("@date_time", DateTime.Now);
                    cmd3.Parameters.AddWithValue("@activity", activityRecord);

                    con.Open();
                    cmd3.ExecuteNonQuery();
                    con.Close();

                    clearData();
                    MessageBox.Show("Account Deleted Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    windowRefresh();

                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                con.Close();
            }

        }

        private void accountsDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = null;
            row_selected = gd.SelectedItem as DataRowView;

            if (row_selected != null)
            { 
                    idTxt.Text =    row_selected["id"].ToString();
                    nameTxt.Text = row_selected["name"].ToString();
                    passTxt.Text = row_selected["password"].ToString();
                    roleCombobox.SelectedValue = row_selected["role"].ToString();
                    questionCombobox.SelectedValue = row_selected["question"].ToString();
                    answerTxt.Text = row_selected["answer"].ToString();
            }
            confirmBtn.Content = "Update";
            confirmBtn.Click -= insertBtn;
            confirmBtn.Click += updateBtn;
        }


        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            LiveTimeLabel.Content = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss tt");
        }

        private void menuBtn(object sender, RoutedEventArgs e)
        {
            Admin_Homepage adhmpg = new Admin_Homepage();
            adhmpg.Owner = this;
            this.Hide();
            adhmpg.Show();
        }

        private void refreshBtn(object sender, RoutedEventArgs e)
        {
            windowRefresh();
        }

        public void windowRefresh()
        {
            Admin_Accounts refresh = new Admin_Accounts();
            refresh.Owner = this;
            this.Hide();
            refresh.Show();
        }

        public void clearData()
        {
            idTxt.Clear();
            nameTxt.Clear();
            passTxt.Clear();
            roleCombobox.Items.Clear();
            questionCombobox.Items.Clear();
            answerTxt.Clear();
        }

        public bool isValid()
        {
            if (nameTxt.Text == string.Empty)
            {
                MessageBox.Show("Employee Name is required.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (passTxt.Text == string.Empty)
            {
                MessageBox.Show("Password is required.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (answerTxt.Text == string.Empty)
            {
                MessageBox.Show("Answer cannot be blank.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (roleCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Role", "Error");
                return false;
            }
            if (questionCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Select a Question", "Error");
                return false;
            }

            return true;
        }

        private void searchBar(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Format("name like '%{0}%'", searchTxt.Text);
                accountsDatagrid.ItemsSource = dv;
            }
        }

        private void allAccounts(object sender, RoutedEventArgs e)
        {
            windowRefresh();
        }

        private void userAccounts(object sender, RoutedEventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = string.Format("role like 'User'", searchTxt.Text);
            accountsDatagrid.ItemsSource = dv;
        }

        private void adminAccounts(object sender, RoutedEventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = string.Format("role like 'Admin'", searchTxt.Text);
            accountsDatagrid.ItemsSource = dv;
        }
    }
}
