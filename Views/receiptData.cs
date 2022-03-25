using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace posproj.Views
{
    class receiptData
    {
        static string productName;
        static int productQuantity;
        static float productPrice;
        static float productAmount;
        static int total;
        static float productCash;
        static float productChange;

        public static string ProductName
        {
            get
            {
                return productName;
            }
            set
            {
                productName = value;
            }
        }

        public static int ProductQuantity
        {
            get
            {
                return productQuantity;
            }
            set
            {
                productQuantity = value;
            }
        }

        public static float ProductPrice
        {
            get
            {
                return productPrice;
            }
            set
            {
                productPrice = value;
            }
        }

        public static float ProductAmount
        {
            get
            {
                return productAmount;
            }
            set
            {
                productAmount = value;
            }
        }

        public static int Total
        {
            get
            {
                return total;
            }
            set
            {
                total = value;
            }
        }

        public static float ProductCash
        {
            get
            {
                return productCash;
            }
            set
            {
                productCash = value;
            }
        }

        public static float ProductChange
        {
            get
            {
                return productChange;
            }
            set
            {
                productChange = value;
            }
        }
    }
}
