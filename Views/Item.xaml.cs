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
    /// <summary>
    /// Interaction logic for Item.xaml
    /// </summary>
    public partial class Item : UserControl
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;
        SqlDataAdapter da;

        public Item()
        {
            InitializeComponent();
            show();
            categoryItems();

        }

        private void refreshBtn(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void refresh()
        {
            InitializeComponent();
            Content = new itemModels();
        }

        DataTable dt = new DataTable();

        private void show()
        {   
            da = new SqlDataAdapter("Select pc.name, pi.itemname, pi.id from posproject.category pc INNER JOIN posproject.inventory_items pi ON pc.id = pi.category_id  WHERE pi.isActive = 'Yes' ", con);

            da.Fill(dt);
            itemDatagrid.ItemsSource = dt.DefaultView;

        }

        void categoryItems()
        {
            
                con.Open();
                cmd = new SqlCommand("Select * from posproject.category ", con);
                SqlDataReader rd = cmd.ExecuteReader();

                while (rd.Read())
                {
                    string categoryName = rd.GetString(1);
                    categoryCombobox.Items.Add(categoryName);
                }

                con.Close();
            

        }


        public bool isValid()
        {
            if (categoryCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Category is required.", "Failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

                    String selectedCategory = categoryCombobox.Text;
                    String categoryId = "";

                    con.Open();

                    SqlCommand selected = new SqlCommand("Select id from posproject.category Where name=@name", con);
                    selected.Parameters.AddWithValue("@name", categoryCombobox.Text);
                    SqlDataReader rd = selected.ExecuteReader();


                    while (rd.Read())
                    {
                        categoryId = rd[0].ToString();
                    }

                    con.Close();

                    cmd = new SqlCommand("Insert into posproject.inventory_items(category_id,itemname) Values (@category_id,@itemname)", con);
                    cmd.Parameters.AddWithValue("@category_id", categoryId);
                    cmd.Parameters.AddWithValue("@itemname", nameTxt.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    clearData();
                    MessageBox.Show("Item Saved Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    String selectedCategory = categoryCombobox.Text;
                    String categoryId = "";

                    con.Open();

                    SqlCommand selected = new SqlCommand("Select id from posproject.category Where name=@name", con);
                    selected.Parameters.AddWithValue("@name", categoryCombobox.Text);
                    SqlDataReader rd = selected.ExecuteReader();


                    while (rd.Read())
                    {
                        categoryId = rd[0].ToString();
                    }

                    con.Close();

                    cmd = new SqlCommand("Update posproject.inventory_items Set category_id=@category_id, itemname=@itemname Where id=@id", con);
                    cmd.Parameters.AddWithValue("@id", idTxt.Text);
                    cmd.Parameters.AddWithValue("@category_id", categoryId);
                    cmd.Parameters.AddWithValue("@itemname", nameTxt.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    clearData();
                    MessageBox.Show("Item Updated Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
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

                    cmd = new SqlCommand("Update posproject.inventory_items Set isActive=@isActive Where id=@id", con);
                    cmd.Parameters.AddWithValue("@id", idTxt.Text);
                    cmd.Parameters.AddWithValue("@isActive", isActive);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    clearData();
                    MessageBox.Show("Item Deleted Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private void itemDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;


            if (row_selected != null)
            {

                idTxt.Text = row_selected["id"].ToString();
                nameTxt.Text = row_selected["itemname"].ToString();
                categoryCombobox.SelectedValue = row_selected["name"].ToString(); ;
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
                dv.RowFilter = string.Format("itemname like '%{0}%'", searchTxt.Text);
                itemDatagrid.ItemsSource = dv;
            }
        }

    }
}

