using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace posproj.Views
{
    class updateQty
    {
        static int inventoryID;
        static int initQTY;
        static int newQTY;

        public static int InventoryID
        {
            get
            {
                return inventoryID;
            }
            set
            {
                inventoryID = value;
            }
        }

        public static int InitQTY
        {
            get
            {
                return initQTY;
            }
            set
            {
                initQTY = value;
            }
        }

        public static int NewQTY
        {
            get
            {
                return newQTY;
            }
            set
            {
                newQTY = value;
            }
        }

    }
}
