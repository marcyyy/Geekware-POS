using posproj.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace posproj.Views
{
    public partial class ItemCard : UserControl
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        SqlCommand cmd;

        public int passId = 0;
        public string passPrice = "";

        public ItemCard()
        {
            InitializeComponent();

            itemId.Content = cardData.ProductId;
            itemCategory.Content = cardData.ProductCategory;
            itemName.Content = cardData.ProductName;
            itemPrice.Content = cardData.ProductPrice;
            pictureBox.Source = new BitmapImage(new Uri(cardData.ProductImage));
        }

        public bool isValid()
        {
            int intQty = 0;

            con.Open();
            SqlCommand cmd100 = new SqlCommand("Select quantity From posproject.inventory Where id=@id", con);
            cmd100.Parameters.AddWithValue("@id", itemId.Content);

            SqlDataReader rda2 = cmd100.ExecuteReader();

            while (rda2.Read())
            {
                intQty = (int)rda2.GetValue(0);
            }
            con.Close();

            if (intQty < 0)
            {
                MessageBox.Show("There's no more record.", "Failed", MessageBoxButton.OK, MessageBoxImage.Warning);

                return false;
            }
            return true;
        }

        private void getData(object sender, MouseButtonEventArgs e)
        {
            if(isValid())
            {

                    string selectedPrice = itemPrice.Content.ToString();
                    string ref_num = generateCode.transacCode;

                    con.Open();
                    SqlCommand cmd0 = new SqlCommand("Select quantity, amount From posproject.order_items Where inventory_id=@inventory_id AND ref_num=@ref_num", con);
                    cmd0.Parameters.AddWithValue("@inventory_id", itemId.Content);
                    cmd0.Parameters.AddWithValue("@ref_num", ref_num);

                    int newQty = 0;
                    string newAmnt = "";

                    SqlDataReader rd = cmd0.ExecuteReader();
                    bool hasRows = (rd.HasRows);

                    while (rd.Read())
                    {
                        newQty = (int)rd.GetValue(0);
                        newAmnt = rd.GetValue(1).ToString();

                    }
                    con.Close();

                    //If there's an existing record
                    if (hasRows == true)
                    {
                        int totalQty = newQty + 1;
                        float newAmount = float.Parse(newAmnt);
                        float usualAmount = float.Parse(selectedPrice);
                        float totalAmount = newAmount + usualAmount;

                        SqlCommand cmd1 = new SqlCommand("Update posproject.order_items Set quantity=@quantity, amount=@amount Where inventory_id=@inventory_id AND ref_num=@ref_num", con);
                        cmd1.Parameters.AddWithValue("@quantity", totalQty);
                        cmd1.Parameters.AddWithValue("@amount", totalAmount);
                        cmd1.Parameters.AddWithValue("@inventory_id", itemId.Content);
                    cmd1.Parameters.AddWithValue("@ref_num", ref_num);

                    con.Open();
                    cmd1.ExecuteNonQuery();
                    con.Close();
                }
                else
                {
                    int qty = 1;
                    cmd = new SqlCommand("Insert into posproject.order_items(ref_num,inventory_id,quantity,amount) Values (@ref_num,@inventory_id,@quantity,@amount)", con);
                    cmd.Parameters.AddWithValue("@ref_num", ref_num);
                    cmd.Parameters.AddWithValue("@inventory_id", itemId.Content);
                    cmd.Parameters.AddWithValue("@quantity", qty);
                    cmd.Parameters.AddWithValue("@amount", itemPrice.Content);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }

                int quantity = 0;

                con.Open();
                SqlCommand selected = new SqlCommand("Select quantity from posproject.inventory WHERE id = @id ", con);
                selected.Parameters.AddWithValue("@id", itemId.Content);
                SqlDataReader rda = selected.ExecuteReader();

                while (rda.Read())
                {
                    quantity = (int)rda.GetValue(0);
                }
                con.Close();

                int quantity1 = quantity - 1;

                SqlCommand cmd2 = new SqlCommand("Update posproject.inventory SET quantity=@quantity WHERE id = @id ", con);
                cmd2.Parameters.AddWithValue("@quantity", quantity1);
                cmd2.Parameters.AddWithValue("@id", itemId.Content);

                con.Open();
                cmd2.ExecuteNonQuery();
                con.Close();

                //insert inventory logs

                string activityRecord = "Product Sold";
                string changeRecord = "-" + 1;

                SqlCommand cmd5 = new SqlCommand("Insert into posproject.log_inventory(date,inventory_id,activity,change,account_id) Values (@date,@inventory_id,@activity,@change,@account_id)", con);
                cmd5.Parameters.AddWithValue("@date", DateTime.Now);
                cmd5.Parameters.AddWithValue("@inventory_id", itemId.Content);
                cmd5.Parameters.AddWithValue("@activity", activityRecord);
                cmd5.Parameters.AddWithValue("@change", changeRecord);
                cmd5.Parameters.AddWithValue("@account_id", loginInfo.accountID);

                con.Open();
                cmd5.ExecuteNonQuery();
                con.Close();

                /*insert sold record
                con.Open();
                SqlCommand cmd21 = new SqlCommand("Select * from posproject.inventory Where item_id=@item_id AND size=@size AND color=@color AND status=@status ", con);
                cmd21.Parameters.AddWithValue("@item_id", itemId);
                cmd21.Parameters.AddWithValue("@size", sizeTxt.Text);
                cmd21.Parameters.AddWithValue("@color", colorTxt.Text);
                cmd21.Parameters.AddWithValue("@status", statusId);

                SqlDataReader rd2 = cmd21.ExecuteReader();

                while (rd2.Read())
                {
                    newId = rd2.GetValue(0).ToString();
                    inActiveQty = rd2.GetValue(1).ToString();
                }
                con.Close();*/

            }
        }


    }
}
