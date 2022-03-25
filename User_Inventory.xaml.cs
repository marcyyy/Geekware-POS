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
using System.Data.SqlClient;
using System.Data;
using posproj.ViewModels;
using Microsoft.Win32;
using System.IO;

namespace posproj
{
    public partial class User_Inventory : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;
        SqlDataAdapter da;

        public User_Inventory()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            show();
            statusItems();

            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
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
        User_Homepage userhmpg = new User_Homepage();
        userhmpg.Owner = this;
        this.Hide();
        userhmpg.Show();
    }

        private void refreshBtn(object sender, RoutedEventArgs e)
        {
            refresh();
        }

        private void refresh() 
        {
            User_Inventory userinv = new User_Inventory();
            userinv.Owner = this;
            this.Hide();
            userinv.Show();
        }

        DataTable dt = new DataTable();

        private void show()
        {
            da = new SqlDataAdapter("Select pi.id, pi.location, pii.itemname, pc.name, pi.size, pi.color, pi.price, pi.quantity, ps.status, pi.isActive from posproject.category pc JOIN posproject.inventory_items pii ON pc.id = pii.category_id JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.status ps ON ps.id = pi.status  WHERE pi.isActive = 'Yes'", con);

            da.Fill(dt);
            inventoryDatagrid.ItemsSource = dt.DefaultView;
        }

        void statusItems()
        {

            con.Open();
            cmd = new SqlCommand("Select * from posproject.status ", con);
            SqlDataReader rd = cmd.ExecuteReader();

            while (rd.Read())
            {
                string statusName = rd.GetString(1);
                statusCombobox.Items.Add(statusName);
            }

            con.Close();

        }

        String statusId = "";
        String itemId = "";

        public void itemConvert()
        {
            con.Open();

            SqlCommand selected = new SqlCommand("Select id from posproject.inventory_items Where itemname=@name", con);
            selected.Parameters.AddWithValue("@name", itemTxt.Text);
            SqlDataReader rd = selected.ExecuteReader();


            while (rd.Read())
            {
                itemId = rd[0].ToString();
            }

            con.Close();
        }

        public void statusConvert()
        {
            con.Open();

            SqlCommand selected = new SqlCommand("Select id from posproject.status Where status=@name", con);
            selected.Parameters.AddWithValue("@name", statusCombobox.Text);
            SqlDataReader rd = selected.ExecuteReader();


            while (rd.Read())
            {
                statusId = rd[0].ToString();
            }

            con.Close();
        }

        public bool isValid()
        {
            if (idTxt.Text == string.Empty)
            {
                MessageBox.Show("Select product first", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (quantityTxt.Text == string.Empty)
            {
                MessageBox.Show("Quantity cannot be blank", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (statusCombobox.SelectedIndex == -1)
            {
                MessageBox.Show("Status cannot be blank", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if(statusCombobox.Text != "Good Condition")
            {
                int initQty = Int32.Parse(qtyTxt.Text);
                int compQty = Int32.Parse(quantityTxt.Text);

                if (compQty > initQty)
                {
                    MessageBox.Show("Insufficient Quantity", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }

            return true;
        }


        private void updateBtn(object sender, RoutedEventArgs e)
        {
            String statusIdentify = statusCombobox.Text;

            try
            {
                if(isValid())
                {
                    if (statusIdentify == "Good Condition")
                    {
                        itemConvert();
                        statusConvert();

                        cmd = new SqlCommand("Update posproject.inventory Set quantity=@quantity Where id=@id", con);
                        cmd.Parameters.AddWithValue("@id", idTxt.Text);
                        cmd.Parameters.AddWithValue("@quantity", quantityTxt.Text);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        //insert inventory logs

                        int initialQty = Int16.Parse(qtyTxt.Text);
                        int changeQty = Int16.Parse(quantityTxt.Text);

                        int totalQty = changeQty - initialQty;
                        string stringQty = totalQty.ToString();

                        string activityRecord = "Product Stock in";
                        string changeRecord = "+" + stringQty;

                        SqlCommand cmd10 = new SqlCommand("Insert into posproject.log_inventory(date,inventory_id,activity,change,account_id) Values (@date,@inventory_id,@activity,@change,@account_id)", con);
                        cmd10.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd10.Parameters.AddWithValue("@inventory_id", idTxt.Text);
                        cmd10.Parameters.AddWithValue("@activity", activityRecord);
                        cmd10.Parameters.AddWithValue("@change", changeRecord);
                        cmd10.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                        con.Open();
                        cmd10.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("Product Updated Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                        refresh();
                    }

                    //else if status == Defective etc
                    else
                    {
                        String isActive = "No";
                        int initialQty = Int16.Parse(qtyTxt.Text);
                        int changeQty = Int16.Parse(quantityTxt.Text);

                        int totalQty = initialQty - changeQty;
                        string stringQty = totalQty.ToString();

                        itemConvert();
                        statusConvert();

                        //Subtract change to good condition products
                        cmd = new SqlCommand("Update posproject.inventory Set quantity=@quantity Where id=@id", con);
                        cmd.Parameters.AddWithValue("@id", idTxt.Text);
                        cmd.Parameters.AddWithValue("@quantity", stringQty);

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();

                        //check if theres an existing record for inActives
                        con.Open();
                        SqlCommand cmd0 = new SqlCommand("Select id, quantity From posproject.inventory Where item_id=@item_id AND size=@size AND color=@color AND status=@status ", con);
                        cmd0.Parameters.AddWithValue("@item_id", itemId);
                        cmd0.Parameters.AddWithValue("@size", sizeTxt.Text);
                        cmd0.Parameters.AddWithValue("@color", colorTxt.Text);
                        cmd0.Parameters.AddWithValue("@status", statusId);

                        string newId = "";
                        string inActiveQty = "";

                        SqlDataReader rd = cmd0.ExecuteReader();
                        bool hasRows = (rd.HasRows);

                        while (rd.Read())
                        {
                            newId = rd.GetValue(0).ToString();
                            inActiveQty = rd.GetValue(1).ToString();
                        }
                        con.Close();

                        //If there's an existing record
                        if (hasRows == true)
                        {
                            int initialQty2 = Int16.Parse(inActiveQty);
                            int changeQty2 = Int16.Parse(quantityTxt.Text);

                            int totalQty2 = initialQty2 + changeQty2;
                            string stringQty2 = totalQty2.ToString();

                            SqlCommand cmd3 = new SqlCommand("Update posproject.inventory Set quantity=@quantity Where id=@id", con);
                            cmd3.Parameters.AddWithValue("@id", newId);
                            cmd3.Parameters.AddWithValue("@quantity", stringQty2);

                            con.Open();
                            cmd3.ExecuteNonQuery();
                            con.Close();

                            //insert inventory logs

                            if (statusIdentify == "Defective")
                            {
                                string activityRecord3 = "Product Defective";
                                string changeRecord3 = "-" + quantityTxt.Text;

                                SqlCommand cmd12 = new SqlCommand("Insert into posproject.log_inventory(date,inventory_id,activity,change,account_id) Values (@date,@inventory_id,@activity,@change,@account_id)", con);
                                cmd12.Parameters.AddWithValue("@date", DateTime.Now);
                                cmd12.Parameters.AddWithValue("@inventory_id", idTxt.Text);
                                cmd12.Parameters.AddWithValue("@activity", activityRecord3);
                                cmd12.Parameters.AddWithValue("@change", changeRecord3);
                                cmd12.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                                con.Open();
                                cmd12.ExecuteNonQuery();
                                con.Close();
                            }
                            else
                            {
                                string activityRecord3 = "Product Sold";
                                string changeRecord3 = "-" + quantityTxt.Text;

                                SqlCommand cmd12 = new SqlCommand("Insert into posproject.log_inventory(date,inventory_id,activity,change,account_id) Values (@date,@inventory_id,@activity,@change,@account_id)", con);
                                cmd12.Parameters.AddWithValue("@date", DateTime.Now);
                                cmd12.Parameters.AddWithValue("@inventory_id", idTxt.Text);
                                cmd12.Parameters.AddWithValue("@activity", activityRecord3);
                                cmd12.Parameters.AddWithValue("@change", changeRecord3);
                                cmd12.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                                con.Open();
                                cmd12.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                        //if there isn't an existing record
                        else
                        {
                            SqlCommand cmd2 = new SqlCommand("Insert into posproject.inventory(item_id,location,size,color,quantity,status,price,isActive) Values (@item_id,@location,@size,@color,@quantity,@status,@price,@isActive)", con);
                            cmd2.Parameters.AddWithValue("@item_id", itemId);
                            cmd2.Parameters.AddWithValue("@location", imgLocation);
                            cmd2.Parameters.AddWithValue("@size", sizeTxt.Text);
                            cmd2.Parameters.AddWithValue("@color", colorTxt.Text);
                            cmd2.Parameters.AddWithValue("@quantity", quantityTxt.Text);
                            cmd2.Parameters.AddWithValue("@status", statusId);
                            cmd2.Parameters.AddWithValue("@price", priceTxt.Text);
                            cmd2.Parameters.AddWithValue("@isActive", isActive);

                            con.Open();
                            cmd2.ExecuteNonQuery();
                            con.Close();

                            //insert inventory logs

                            string activityRecord2 = "Product Inactive";
                            string changeRecord2 = "-" + quantityTxt.Text;

                            SqlCommand cmd11 = new SqlCommand("Insert into posproject.log_inventory(date,inventory_id,activity,change,account_id) Values (@date,@inventory_id,@activity,@change,@account_id)", con);
                            cmd11.Parameters.AddWithValue("@date", DateTime.Now);
                            cmd11.Parameters.AddWithValue("@inventory_id", idTxt.Text);
                            cmd11.Parameters.AddWithValue("@activity", activityRecord2);
                            cmd11.Parameters.AddWithValue("@change", changeRecord2);
                            cmd11.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                            con.Open();
                            cmd11.ExecuteNonQuery();
                            con.Close();
                        }
                        con.Close();

                        MessageBox.Show("Product Updated Successfully.", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
                        refresh();
                    }

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

        String imgLocation = "";

        private void itemDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = gd.SelectedItem as DataRowView;

            imgLocation = row_selected?["location"].ToString();

            if (row_selected != null)
            {
                pictureBox.Source = new BitmapImage(new Uri(imgLocation));
                idTxt.Text = row_selected["id"].ToString();
                itemTxt.Text = row_selected["itemname"].ToString();
                categoryTxt.Text = row_selected["name"].ToString();
                sizeTxt.Text = row_selected["size"].ToString();
                colorTxt.Text = row_selected["color"].ToString();
                statusCombobox.SelectedValue = row_selected["status"].ToString();
                quantityTxt.Text = row_selected["quantity"].ToString();
                qtyTxt.Text = row_selected["quantity"].ToString();
                priceTxt.Text = row_selected["price"].ToString();

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
                inventoryDatagrid.ItemsSource = dv;
            }
        }

        private void insertBtn(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Select a Product first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
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
