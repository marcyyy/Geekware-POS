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
    public partial class Refund_Main : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        //SqlCommand cmd;
        SqlDataAdapter da;

        DataTable dt = new DataTable();
        string transacNo = "";

        public Refund_Main()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();

            transacNo = refundData.RefundNum;
            newAmountTxt.Text = (refundData.RefundAmount).ToString();
            show();

            refQtyTxt.Text = "0";
            qtyTxt.Text = "0";
            priceTxt.Text = "0";
            amountTxt.Text = "0";

        }

        private void show()
        {
            da = new SqlDataAdapter("Select poi.id, poi.ref_num, poi.inventory_id, pii.itemname, pi.price, poi.quantity, poi.amount, pot.total FROM posproject.inventory_items pii JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.order_items poi ON pi.id = poi.inventory_id JOIN posproject.order_transaction pot ON poi.ref_num = pot.ref_num WHERE poi.ref_num = '"+ transacNo +"' AND poi.amount > 0 ", con);

            da.Fill(dt);
            itemDatagrid.ItemsSource = dt.DefaultView;

        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void refundBtn(object sender, RoutedEventArgs e)
        {
            String reasonIdentify = reasonCombobox.Text;
            int prodQty = 0;

            con.Open();
            SqlCommand cmd0 = new SqlCommand("Select quantity From posproject.inventory Where id=@id ", con);
            cmd0.Parameters.AddWithValue("@id", prodIdTxt.Text);
            SqlDataReader rd = cmd0.ExecuteReader();
            bool hasRows = (rd.HasRows);

            while (rd.Read())
            {
                prodQty = (int)rd.GetValue(0);
            }
            con.Close();

            try
            {
                if (isValid())
                {
                    int initQty = Int32.Parse(qtyTxt.Text);
                    int refQty = Int32.Parse(refQtyTxt.Text);
                    int price = Int32.Parse(priceTxt.Text);
                    int initAmount = Int32.Parse(amountTxt.Text);

                    int newQty = initQty - refQty;
                    int initRef = refQty * price;
                    int newAmount = initAmount - initRef;

                    refundData.RefundAmount = initRef + refundData.RefundAmount;

                    //update order_items
                    SqlCommand cmd1 = new SqlCommand("Update posproject.order_items SET quantity=@quantity, amount=@amount WHERE ref_num=@ref_num AND inventory_id = @inventory_id ", con);
                    cmd1.Parameters.AddWithValue("@quantity", newQty);
                    cmd1.Parameters.AddWithValue("@amount", newAmount);
                    cmd1.Parameters.AddWithValue("@ref_num", transTxt.Text);
                    cmd1.Parameters.AddWithValue("@inventory_id", prodIdTxt.Text);

                    con.Open();
                    cmd1.ExecuteNonQuery();
                    con.Close();

                    //insert log_orders
                    string activityRecord1 = "Refund Transaction";

                    SqlCommand cmd4 = new SqlCommand("Insert into posproject.log_orders(date,ref_num,activity,account_id) VALUES(@date,@ref_num,@activity,@account_id)", con);
                    cmd4.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd4.Parameters.AddWithValue("@ref_num", transTxt.Text);
                    cmd4.Parameters.AddWithValue("@activity", activityRecord1);
                    cmd4.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                    con.Open();
                    cmd4.ExecuteNonQuery();
                    con.Close();

                    //insert log_inventory
                    if (reasonIdentify == "Item Defective")
                    {
                        string activityRecord = "Product Refunded";
                        string changeRecord = "Defective (" + refQtyTxt.Text + ")";

                        SqlCommand cmd2 = new SqlCommand("Insert into posproject.log_inventory(date,inventory_id,activity,change,account_id) VALUES(@date,@inventory_id,@activity,@change,@account_id)", con);
                        cmd2.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd2.Parameters.AddWithValue("@inventory_id", prodIdTxt.Text);
                        cmd2.Parameters.AddWithValue("@activity", activityRecord);
                        cmd2.Parameters.AddWithValue("@change", changeRecord);
                        cmd2.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                        con.Open();
                        cmd2.ExecuteNonQuery();
                        con.Close();

                    }
                    else
                    {
                        string activityRecord = "Product Refunded";
                        string changeRecord = "+" + refQtyTxt.Text;

                        SqlCommand cmd3 = new SqlCommand("Insert into posproject.log_inventory(date,inventory_id,activity,change,account_id) VALUES(@date,@inventory_id,@activity,@change,@account_id)", con);
                        cmd3.Parameters.AddWithValue("@date", DateTime.Now);
                        cmd3.Parameters.AddWithValue("@inventory_id", prodIdTxt.Text);
                        cmd3.Parameters.AddWithValue("@activity", activityRecord);
                        cmd3.Parameters.AddWithValue("@change", changeRecord);
                        cmd3.Parameters.AddWithValue("@account_id", loginInfo.accountID);


                        con.Open();
                        cmd3.ExecuteNonQuery();
                        con.Close();

                        //update inventory

                        int totalQty = prodQty + refQty;
                        string stringQty = totalQty.ToString();

                        SqlCommand cmd21 = new SqlCommand("Update posproject.inventory Set quantity=@quantity Where id=@id", con);
                        cmd21.Parameters.AddWithValue("@quantity", stringQty);
                        cmd21.Parameters.AddWithValue("@id", prodIdTxt.Text);


                        con.Open();
                        cmd21.ExecuteNonQuery();
                        con.Close();
                    }

                    //insert refund dbs

                    SqlCommand cmd20 = new SqlCommand("Insert into posproject.order_refund(ref_num,inventory_id,quantity,amount) Values (@ref_num,@inventory_id,@quantity,@amount)", con);
                    cmd20.Parameters.AddWithValue("@ref_num", refundData.RefundNum);
                    cmd20.Parameters.AddWithValue("@inventory_id", prodIdTxt.Text);
                    cmd20.Parameters.AddWithValue("@quantity", refQtyTxt.Text);
                    cmd20.Parameters.AddWithValue("@amount", initRef);

                    con.Open();
                    cmd20.ExecuteNonQuery();
                    con.Close();

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

            refreshContent();
        }

        void refreshContent()
        {
            Refund_Main refRefresh = new Refund_Main();
            refRefresh.Owner = this;
            this.Hide();
            refRefresh.Show();
        }

        private void itemDatagrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid gd = (DataGrid)sender;
            DataRowView row_selected = null;
            row_selected = gd.SelectedItem as DataRowView;

            if (row_selected != null)
            {
                idTxt.Text = row_selected["id"].ToString();
                transTxt.Text = row_selected["ref_num"].ToString();
                prodIdTxt.Text = row_selected["inventory_id"].ToString();
                prodNameTxt.Text = row_selected["itemname"].ToString();
                qtyTxt.Text = row_selected["quantity"].ToString();
                priceTxt.Text = row_selected["price"].ToString();
                amountTxt.Text = row_selected["amount"].ToString();
            }
        }

        public bool isValid()
        {
            if (idTxt.Text == string.Empty )
            {
                MessageBox.Show("Please select a record", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (qtyTxt.Text == string.Empty)
            {
                MessageBox.Show("Please select a record", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (refQtyTxt.Text == string.Empty )
            {
                MessageBox.Show("Enter Refund Qty", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            if (reasonCombobox.SelectedIndex == -1)//Nothing selected
            {
                MessageBox.Show("Enter Refund Reason", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            int initQty = Int32.Parse(qtyTxt.Text);
            int compQty = Int32.Parse(refQtyTxt.Text);

            if (compQty > initQty)
            {
                MessageBox.Show("Insufficient Quantity", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            return true;
        }

        private void doneBtn(object sender, RoutedEventArgs e)
        {
            int totalAmt = 0;

            con.Open();
            SqlCommand cmd90 = new SqlCommand("Select total from posproject.order_transaction ", con);
            SqlDataReader rd = cmd90.ExecuteReader();

            while (rd.Read())
            {
                totalAmt = (int)rd.GetValue(0);
            }

            con.Close();

            int chAmt = refundData.RefundAmount;
            int total = totalAmt - chAmt;

            //update order_transaction

            String actRef1 = "No";

            SqlCommand cmd14 = new SqlCommand("Update posproject.order_transaction SET isActive = '" + actRef1 + "' Where ref_num = @ref_num ", con);
            cmd14.Parameters.AddWithValue("@ref_num", refundData.RefundNum);

            con.Open();
            cmd14.ExecuteNonQuery();
            con.Close();

            //insert into order_transaction

            String actRef2 = "Sell (Updated)";

            SqlCommand cmd13 = new SqlCommand("Insert into posproject.order_transaction(ref_num, order_date, activity, total) Values (@ref_num, @order_date, @activity, @total)", con);
            cmd13.Parameters.AddWithValue("@ref_num", refundData.RefundNum);
            cmd13.Parameters.AddWithValue("@order_date", DateTime.Now);
            cmd13.Parameters.AddWithValue("@activity", actRef2);
            cmd13.Parameters.AddWithValue("@total", total);

            con.Open();
            cmd13.ExecuteNonQuery();
            con.Close();

            //insert into order_transaction

            String actRef = "Refund";

            SqlCommand cmd11 = new SqlCommand("Insert into posproject.order_transaction(ref_num, order_date, activity, total) Values (@ref_num, @order_date, @activity, @total)", con);
            cmd11.Parameters.AddWithValue("@ref_num", refundData.RefundNum);
            cmd11.Parameters.AddWithValue("@order_date", DateTime.Now);
            cmd11.Parameters.AddWithValue("@activity", actRef);
            cmd11.Parameters.AddWithValue("@total", refundData.RefundAmount);

            con.Open();
            cmd11.ExecuteNonQuery();
            con.Close();

            Refund_Amount refund = new Refund_Amount();
            refund.Owner = this;
            this.Hide();
            refund.Show();
        }

        private void refQtyTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(refQtyTxt.Text, "[^0-9]"))
            {
                MessageBox.Show("Please enter only numbers.");
                refQtyTxt.Text = refQtyTxt.Text.Remove(refQtyTxt.Text.Length - 1);
            }
        }
    }
}
