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
using Microsoft.Win32;
using System.IO;
using System.Globalization;

namespace posproj.Views
{
    public partial class Inventory : UserControl
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;
        SqlDataAdapter da;

        public Inventory()
        {
            InitializeComponent();
            show();
            itemItems();

        }

        private void refreshBtn(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void refresh() 
        {
            InitializeComponent();
            Content = new inventoryModels();
        }

        DataTable dt = new DataTable();

        private void show()
        {
            da = new SqlDataAdapter("Select pi.id, pi.location, pii.itemname, pc.name, pi.size, pi.color, pi.price, pi.quantity, ps.status from posproject.category pc JOIN posproject.inventory_items pii ON pc.id = pii.category_id JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.status ps ON ps.id = pi.status  WHERE pi.isActive = 'Yes'", con);

            da.Fill(dt);
            inventoryDatagrid.ItemsSource = dt.DefaultView;
        }

        void itemItems()
        {

            con.Open();
            cmd = new SqlCommand("Select * from posproject.inventory_items ", con);
            SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                string itemName = rd.GetString(2);
                itemCombobox.Items.Add(itemName);
            }

            con.Close();

        }

        void itemCategory()
        {
            String categoryId = "";
            String categoryName = "";
            SqlDataReader rd;

            con.Open();

            SqlCommand selected = new SqlCommand("Select category_id from posproject.inventory_items Where itemname=@name", con);
            selected.Parameters.AddWithValue("@name", itemCombobox.Text);
            rd = selected.ExecuteReader();

            while (rd.Read())
            {
                categoryId = rd[0].ToString();
            }
            con.Close();
            con.Open();

            SqlCommand category = new SqlCommand("Select name from posproject.category Where id=@id", con);
            category.Parameters.AddWithValue("@id", categoryId);
            rd = category.ExecuteReader();

            while (rd.Read())
            {
                categoryName = rd[0].ToString();
            }

            categoryTxt.Text = categoryName;

            con.Close();

        }

        public bool isValid()
        {
            if (itemCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Select Item Name", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (priceTxt.Text == string.Empty)
            {
                MessageBox.Show("Quantity cannot be blank", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (quantityTxt.Text == string.Empty)
            {
                MessageBox.Show("Quantity cannot be blank", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (imgLocation == string.Empty)
            {
                MessageBox.Show("Image Required", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        public void clearData()
        {
            sizeTxt.Clear();
            colorTxt.Clear();
            quantityTxt.Clear();

        }

        String itemId = "";
        String statusId = "";

        public void itemConvert()
        {
            con.Open();

            SqlCommand selected = new SqlCommand("Select id from posproject.inventory_items Where itemname=@name", con);
            selected.Parameters.AddWithValue("@name", itemCombobox.Text);
            SqlDataReader rd = selected.ExecuteReader();


            while (rd.Read())
            {
                itemId = rd[0].ToString();
            }

            con.Close();
        }

        public void statusConvert()
        {
            String statusTxt = "Good Condition";
            con.Open();

            SqlCommand selected = new SqlCommand("Select id from posproject.status Where status=@name", con);
            selected.Parameters.AddWithValue("@name", statusTxt);
            SqlDataReader rd = selected.ExecuteReader();


            while (rd.Read())
            {
                statusId = rd[0].ToString();
            }

            con.Close();
        }

        String imgLocation = "";

        private void browseBtn(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG Files|*.png|" + "JPG Files|*.jpg|" + "All files|*.*";
            if (dialog.ShowDialog() == true)
            {
                imgLocation = dialog.FileName.ToString();
                pictureBox.Source = new BitmapImage(new Uri(imgLocation));
            }

        }

        private void insertBtn(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isValid())
                {
                    byte[] images = null;
                    FileStream streem = new FileStream(imgLocation, FileMode.Open, FileAccess.Read);
                    BinaryReader brs = new BinaryReader(streem);
                    images = brs.ReadBytes((int)streem.Length);

                    itemConvert();
                    statusConvert();

                    cmd = new SqlCommand("Insert into posproject.inventory(item_id,image,location,size,color,quantity,status,price) Values (@item_id,@image,@location,@size,@color,@quantity,@status,@price) ;SELECT SCOPE_IDENTITY();", con);
                    cmd.Parameters.AddWithValue("@item_id", itemId);
                    cmd.Parameters.AddWithValue("@image", images);
                    cmd.Parameters.AddWithValue("@location", imgLocation);
                    cmd.Parameters.AddWithValue("@size", sizeTxt.Text);
                    cmd.Parameters.AddWithValue("@color", colorTxt.Text);
                    cmd.Parameters.AddWithValue("@quantity", quantityTxt.Text);
                    cmd.Parameters.AddWithValue("@status", statusId);
                    cmd.Parameters.AddWithValue("@price", priceTxt.Text);

                    con.Open();
                    int savedId = Convert.ToInt32(cmd.ExecuteScalar());
                    con.Close();


                    string activityRecord = "Product Registered";
                    string changeRecord = "+" + quantityTxt.Text;

                    SqlCommand cmd1 = new SqlCommand("Insert into posproject.log_inventory(date,inventory_id,activity,change,account_id) Values (@date,@inventory_id,@activity,@change,@account_id)", con);
                    cmd1.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd1.Parameters.AddWithValue("@inventory_id", savedId);
                    cmd1.Parameters.AddWithValue("@activity", activityRecord);
                    cmd1.Parameters.AddWithValue("@change", changeRecord);
                    cmd1.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                    con.Open();
                    cmd1.ExecuteNonQuery();
                    con.Close();

                    clearData();
                    MessageBox.Show("Product Saved Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);

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
                    itemConvert();
                    statusConvert();

                    cmd = new SqlCommand("Update posproject.inventory Set item_id=@item_id, size=@size, color=@color, quantity=@quantity, status=@status, price=@price Where id=@id", con);
                    cmd.Parameters.AddWithValue("@id", idTxt.Text);
                    cmd.Parameters.AddWithValue("@item_id", itemId);
                    //cmd.Parameters.AddWithValue("@location", locationTxt.Text);
                    //cmd.Parameters.AddWithValue("@image", imageTxt.Text);
                    cmd.Parameters.AddWithValue("@size", sizeTxt.Text);
                    cmd.Parameters.AddWithValue("@color", colorTxt.Text);
                    cmd.Parameters.AddWithValue("@quantity", quantityTxt.Text);
                    cmd.Parameters.AddWithValue("@status", statusId);
                    cmd.Parameters.AddWithValue("@price", priceTxt.Text);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    int initialQty = Int16.Parse(qtyTxt.Text);
                    int changeQty = Int16.Parse(quantityTxt.Text);

                    int totalQty = changeQty - initialQty;
                    string stringQty = totalQty.ToString();

                    string activityRecord = "Product Stock in";
                    string changeRecord = "+" + stringQty;

                    SqlCommand cmd1 = new SqlCommand("Insert into posproject.log_inventory(date,inventory_id,activity,change,account_id) Values (@date,@inventory_id,@activity,@change,@account_id)", con);
                    cmd1.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd1.Parameters.AddWithValue("@inventory_id", idTxt.Text);
                    cmd1.Parameters.AddWithValue("@activity", activityRecord);
                    cmd1.Parameters.AddWithValue("@change", changeRecord);
                    cmd1.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                    con.Open();
                    cmd1.ExecuteNonQuery();
                    con.Close();

                    clearData();
                    MessageBox.Show("Product Updated Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);

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

            imgLocation = row_selected?["location"].ToString();

            if (row_selected != null)
            {
                pictureBox.Source = new BitmapImage(new Uri(imgLocation));
                idTxt.Text = row_selected["id"].ToString();
                itemCombobox.SelectedValue = row_selected["itemname"].ToString();
                categoryTxt.Text = row_selected["name"].ToString();
                sizeTxt.Text = row_selected["size"].ToString();
                colorTxt.Text = row_selected["color"].ToString();
                quantityTxt.Text = row_selected["quantity"].ToString();
                priceTxt.Text = row_selected["price"].ToString();
                qtyTxt.Text = row_selected["quantity"].ToString();
            }

            plusBtn.Content = "+";
            plusBtn.Click -= quantityBtn;
            plusBtn.Click += addBtn;
            confirmBtn.Content = "Update";
            confirmBtn.Click -= insertBtn;
            confirmBtn.Click += updateBtn;
            quantityTxt.IsReadOnly = true;
            browse.Visibility = Visibility.Hidden;
        }

        private void searchBar(object sender, KeyEventArgs e)
        {
            if (e.Key >= Key.A && e.Key <= Key.Z)
            {
                DataView dv = dt.DefaultView;
                dv.RowFilter = string.Format("itemname like '%{0}%'", searchTxt.Text);
                inventoryDatagrid.ItemsSource = dv;
            }
        }

        private void itemCombobox_DropDownClosed(object sender, EventArgs e)
        {
            itemCategory();
        }

        private void allAccounts(object sender, RoutedEventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = string.Format("isActive like 'Yes'", searchTxt.Text);
            inventoryDatagrid.ItemsSource = dv;
        }

        private void adminAccounts(object sender, RoutedEventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = string.Format("isActive like 'No'", searchTxt.Text);
            inventoryDatagrid.ItemsSource = dv;
        }

        private void quantityBtn(object sender, RoutedEventArgs e)
        {

        }

        private void addBtn(object sender, RoutedEventArgs e)
        {
            int initialQty = Int16.Parse(quantityTxt.Text);
            int totalQty = 0;

            stockIn addStock = new stockIn();
            if (addStock.ShowDialog() == false)
            {
                int addQty = Int16.Parse(addStock.addVal);
                totalQty = initialQty + addQty;
                string stringQty = totalQty.ToString();

                quantityTxt.Text = stringQty;

            }
        }

        private void priceTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(priceTxt.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                priceTxt.Text = priceTxt.Text.Remove(priceTxt.Text.Length - 1);
            }
        }

        private void quantityTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(quantityTxt.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                quantityTxt.Text = quantityTxt.Text.Remove(quantityTxt.Text.Length - 1);
            }
        }
    }
}