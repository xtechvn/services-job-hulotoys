using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.API
{
    public class VietQRBankDetail
    {
        public int id { get; set; }
        public string name { get; set; }
        public string code { get; set; }
        public string bin { get; set; }
        public string shortName { get; set; }
        public string logo { get; set; }
        public string short_name { get; set; }
        public string swift_code { get; set; }
    }
    public class ImageBase64
    {
        public string ImageData { get; set; }
        public string ImageExtension { get; set; }
    }
}
