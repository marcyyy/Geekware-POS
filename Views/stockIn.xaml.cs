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

namespace posproj.Views
{
    public partial class stockIn : Window
    {
        private string stockVal;

        public string addVal
        {
            get { return stockVal; }
            set { stockVal = value; }
        }

        public stockIn()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void stockinBtn(object sender, RoutedEventArgs e)
        {
            addVal = stockinTxt.Text;
            this.Close();
        }

        private void CloseBtn(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
