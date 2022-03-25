using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace posproj.Views
{
    class cancelOrder
    {
        static int inventoryId;
        static int quantity;

        public static int InventoryId
        {
            get
            {
                return inventoryId;
            }
            set
            {
                inventoryId = value;
            }
        }

        public static int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                quantity = value;
            }
        }


    }
}
