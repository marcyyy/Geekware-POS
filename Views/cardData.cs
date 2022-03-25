using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace posproj.Views
{
    class cardData
    {
        static int productId;
        static string productCategory;
        static string productName;
        static string productImage;
        static string productPrice;

        public static int ProductId
        {
            get
            {
                return productId;
            }
            set
            {
                productId = value;
            }
        }

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

        public static string ProductCategory
        {
            get
            {
                return productCategory;
            }
            set
            {
                productCategory = value;
            }
        }

        public static string ProductImage
        {
            get
            {
                return productImage;
            }
            set
            {
                productImage = value;
            }
        }

        public static string ProductPrice
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

        
    }
}
