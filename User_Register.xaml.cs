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
using posproj.Views;
using posproj.ViewModels;

namespace posproj
{
    public partial class User_Register : Window
    {
        SqlConnection con = new SqlConnection(@"Data Source=DESKTOP-42VD812\SQLEXPRESS;Initial Catalog=POSdbs;Integrated Security=True");
        //SqlCommand cmd;
        SqlDataAdapter da;

        private static Random random = new Random();
        public string transacNum = "";

        public User_Register()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            showReceipt();

            int maxChar = 12;
            transacNum = RandomString(maxChar);
            generateCode.transacCode = transacNum;

            DataContext = new ReceiptList();

            DispatcherTimer LiveTime = new DispatcherTimer();
            LiveTime.Interval = TimeSpan.FromSeconds(1);
            LiveTime.Tick += timer_Tick;
            LiveTime.Start();
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        DataTable dt = new DataTable();

        public void showReceipt()
        {
            da = new SqlDataAdapter("Select poi.id, poi.ref_num, pii.itemname, poi.quantity, poi.amount from posproject.inventory_items pii JOIN posproject.inventory pi ON pii.id = pi.item_id JOIN posproject.order_items poi ON pi.id = poi.inventory_id WHERE poi.ref_num = '" + transacNum + "' ", con);

            da.Fill(dt);
            //receiptDatagrid.ItemsSource = dt.DefaultView;
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

        private void Return_MouseEnter(object sender, MouseEventArgs e)
        {
            popup_uc.PlacementTarget = Return;
            popup_uc.Placement = PlacementMode.Right;
            popup_uc.IsOpen = true;
            Header.PopupText.Text = "Refund";
        }

        private void Return_MouseLeave(object sender, MouseEventArgs e)
        {
            popup_uc.Visibility = Visibility.Collapsed;
            popup_uc.IsOpen = false;
        }

        private void Bill_MouseEnter(object sender, MouseEventArgs e)
        {
            popup_uc.PlacementTarget = Bill;
            popup_uc.Placement = PlacementMode.Right;
            popup_uc.IsOpen = true;
            Header.PopupText.Text = "Refresh";
        }

        private void Bill_MouseLeave(object sender, MouseEventArgs e)
        {
            popup_uc.Visibility = Visibility.Collapsed;
            popup_uc.IsOpen = false;
        }

        private void refundBtn(object sender, RoutedEventArgs e)
        {
            Refund_Auth refundAuth = new Refund_Auth();
            refundAuth.Show();
        }

        private void refreshBtn(object sender, RoutedEventArgs e)
        {
            DataContext = new ReceiptList();
        }

    }
}
