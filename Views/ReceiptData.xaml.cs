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

namespace posproj.Views
{
    /// <summary>
    /// Interaction logic for ReceiptData.xaml
    /// </summary>
    public partial class ReceiptData : UserControl
    {
        public ReceiptData()
        {
            InitializeComponent();

            nameTxt.Text = receiptData.ProductName;
            priceTxt.Text = receiptData.ProductPrice.ToString();
            quantityTxt.Text = receiptData.ProductQuantity.ToString();
            amountTxt.Text = receiptData.ProductAmount.ToString();
        }
    }
}
