using System;

namespace APP.CHECKOUT_SERVICE.ViewModel.Elasticsearch
{
   public class OrderElasticsearchViewModel: APP.CHECKOUT_SERVICE.ViewModel.Order.Order
    {
        public long id { get; set; } // ID ElasticSearch

        public int deposit_type { get; set; }
        public string ImageScreen { get; set; }
        public long? UserVerifyId { get; set; }
        public DateTime? VerifyDate { get; set; }
        public string NoteReject { get; set; }
        public void GenID()
        {
            string datetime = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString() + (new Random().Next(100, 999)).ToString();
            id = Convert.ToInt64(datetime);
        }
    }
}
