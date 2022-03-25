using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace posproj
{
    class refundData
    {
        static string refundNum;
        static int refundAmount;

        public static string RefundNum
        {
            get
            {
                return refundNum;
            }
            set
            {
                refundNum = value;
            }
        }

        public static int RefundAmount
        {
            get
            {
                return refundAmount;
            }
            set
            {
                refundAmount = value;
            }
        }


    }
}
