using System;
using System.Collections.Generic;
using System.Text;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order.MongoDb
{
    public class BookingFlyUnsucessMongoDbModel
    {
        public string _id { get; set; }
        public int client_id { get; set; }
        public int booking_id { get; set; }
        public string? utmmedium { get; set; }
        public string? utm_source { get; set; }
        public string voucher_name { get; set; }

        public string session_id { get; set; }
        public BookingFlyUnsuccessData booking_data { get; set; }
        public BookingFlyOrder booking_order { get; set; }
        public BookingFlySessionString booking_session { get; set; }

        public DateTime create_date { get; set; }
    }
    public class BookingFlyUnsuccessData 
    {
        public int BookingId { get; set; }
        public string OrderCode { get; set; }
        public bool Status { get; set; }
        public List<BookingFlyUnsucessDataBooking> ListBooking { get; set; }
    }

    public class BookingFlyUnsucessDataBooking
    {
        public List<BookingFlyUnsucessDataListFareData> ListFareData { get; set; }

    }
    public class BookingFlyUnsucessDataListFareData
    {
        public string Airline { get; set; }

        public int FareDataId { get; set; }
        public List<BookingFlyDataFareDataListFlight> ListFlight { get; set; }
        public int Leg { get; set; }
        public int Adt { get; set; }
        public int Chd { get; set; }
        public int Inf { get; set; }
        public double FareAdt { get; set; }
        public double FareChd { get; set; }
        public double FareInf { get; set; }
        public double TaxAdt { get; set; }
        public double TaxChd { get; set; }
        public double TaxInf { get; set; }
        public double FeeAdt { get; set; }
        public double FeeChd { get; set; }
        public double FeeInf { get; set; }
        public double ServiceFeeAdt { get; set; }
        public double ServiceFeeChd { get; set; }
        public double ServiceFeeInf { get; set; }
        public double TotalNetPrice { get; set; }
        public double TotalServiceFee { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalCommission { get; set; }
        public double TotalPrice { get; set; }
        public BookingFlyDataFareDataAdavigoPrice AdavigoPrice { get; set; }
    }
   
}
