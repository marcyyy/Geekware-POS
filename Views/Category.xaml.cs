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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using posproj.ViewModels;

namespace posproj.Views
{
    public partial class Category : UserControl
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;
        SqlDataAdapter da;

        public Category()
        {
            InitializeComponent();
            show();

        }

        private void refreshBtn(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void refresh()
        {
            InitializeComponent();
            Content = new Category();
        }

        DataTable dt = new DataTable();

        private void show()
        {
            da = new SqlDataAdapter("Select * from posproject.category  WHERE isActive = 'Yes' ", con);

            da.Fill(dt);
            categoryDatagrid.ItemsSource = dt.DefaultView;
        }

        public bool isValid()
        {
            if (nameTxt.Text == string.Empty)
            {
                MessageBox.Show("Item Name is required.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        public void clearData()
        {
            nameTxt.Clear();
            idTxt.Clear();
        }


        private void insertBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    cmd = new SqlCommand("Insert into posproject.category(name) Values (@name)", con);
                    cmd.Parameters.AddWithValue("@name", nameTxt.Text);


                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    clearData();
                    MessageBox.Show("Category Saved Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    refresh();
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
                    cmd = new SqlCommand("Update posproject.category Set name=@name Where id=@id", con);
                    cmd.Parameters.AddWithValue("@id", idTxt.Text);
                    cmd.Parameters.AddWithValue("@name", nameTxt.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    clearData();
                    MessageBox.Show("Category Updated Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    refresh();
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

                    cmd = new SqlCommand("Update posproject.category Set isActive=@isActive Where id=@id", con);
                    cmd.Parameters.AddWithValue("@id", idTxt.Text);
                    cmd.Parameters.AddWithValue("@isActive", isActive);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    clearData();
                    MessageBox.Show("Category Deleted Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                    refresh();
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

        private void categoryDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;

            if (row_selected != null)
            {

                idTxt.Text = row_selected["id"].ToString();
                nameTxt.Text = row_selected["name"].ToString();
            }

            confirmBtn.Content = "Update";
            confirmBtn.Click -= insertBtn;
            confirmBtn.Click += updateBtn;
        }

        private void searchBar(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Format("name like '%{0}%'", searchTxt.Text);
                categoryDatagrid.ItemsSource = dv;
            }
        }

    }
}
