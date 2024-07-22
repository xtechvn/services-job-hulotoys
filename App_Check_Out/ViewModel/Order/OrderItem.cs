using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace APP.CHECKOUT_SERVICE.ViewModel.Order
{
    public class OrderItem
    {
        public long Id { get; set; }
        public long OrderId { get; set; }
        public long ProductId { get; set; }
        public double Price { get; set; }
        public int? Quantity { get; set; }
        public DateTime? CreateOn { get; set; }
        public DateTime? UpdateLast { get; set; }
        public string ImageThumb { get; set; }
        public long? OrderItempMapId { get; set; }
        public int? ProductType { get; set; }
    }
}
