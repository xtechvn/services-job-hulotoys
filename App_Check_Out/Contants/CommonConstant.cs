using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.Contants
{
    public class CommonConstant
    {
        public enum CommonGender
        {
            MALE = 1,
            FEMALE = 2,
            OTHER = 0
        }

        public enum FlyBookingDetailType
        {
            GO = 0, // chiều đi
            BACK = 1 // chiều về
        }
    }
}
