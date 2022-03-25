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
using posproj.ViewModels;

namespace posproj
{
    /// <summary>
    /// Interaction logic for Admin_Inventory.xaml
    /// </summary>
    public partial class Admin_Inventory : Window
    {
        public Admin_Inventory()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
            DataContext = new inventoryModels();

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
            Admin_Homepage adhmpg = new Admin_Homepage();
            adhmpg.Owner = this;
            this.Hide();
            adhmpg.Show();
        }

        private void Category_MouseEnter(object sender, MouseEventArgs e)
        {
            popup_uc.PlacementTarget = Category;
            popup_uc.Placement = PlacementMode.Right;
            popup_uc.IsOpen = true;
            Header.PopupText.Text = "Category";
        }

        private void Category_MouseLeave(object sender, MouseEventArgs e)
        {
            popup_uc.Visibility = Visibility.Collapsed;
            popup_uc.IsOpen = false;
        }

        private void Item_MouseEnter(object sender, MouseEventArgs e)
        {
            popup_uc.PlacementTarget = Item;
            popup_uc.Placement = PlacementMode.Right;
            popup_uc.IsOpen = true;
            Header.PopupText.Text = "Item";
        }

        private void Item_MouseLeave(object sender, MouseEventArgs e)
        {
            popup_uc.Visibility = Visibility.Collapsed;
            popup_uc.IsOpen = false;
        }

        private void Inventory_MouseEnter(object sender, MouseEventArgs e)
        {
            popup_uc.PlacementTarget = Inventory;
            popup_uc.Placement = PlacementMode.Right;
            popup_uc.IsOpen = true;
            Header.PopupText.Text = "Stock In";
        }

        private void Inventory_MouseLeave(object sender, MouseEventArgs e)
        {
            popup_uc.Visibility = Visibility.Collapsed;
            popup_uc.IsOpen = false;
        }

        private void categoryBtn(object sender, RoutedEventArgs e)
        {
            DataContext = new categoryModels();
        }

        private void itemBtn(object sender, RoutedEventArgs e)
        {
            DataContext = new itemModels();
        }

        private void inventoryBtn(object sender, RoutedEventArgs e)
        {
            DataContext = new inventoryModels();
        }

        private void inventoryUpdateBtn(object sender, RoutedEventArgs e)
        {
            DataContext = new inventoryUpdateModels();
        }

        private void InventoryUpdate_MouseEnter(object sender, MouseEventArgs e)
        {
            popup_uc.PlacementTarget = inventoryUpdate;
            popup_uc.Placement = PlacementMode.Right;
            popup_uc.IsOpen = true;
            Header.PopupText.Text = "Inventory";
        }

        private void InventoryUpdate_MouseLeave(object sender, MouseEventArgs e)
        {
            popup_uc.Visibility = Visibility.Collapsed;
            popup_uc.IsOpen = false;
        }
    }
}
