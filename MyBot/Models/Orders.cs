using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MyBot.Models
{
    public partial class Orders
    {
        public Orders()
        {
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public int? PaymentId { get; set; }
        public string Address { get; set; }
        public string PaypalMethod { get; set; }
        public int? OrderStatus { get; set; }
        public double? ShippingPrice { get; set; }
        public double? TotalPrice { get; set; }
        public bool? IsPaid { get; set; }
        public string PaidAt { get; set; }
        public bool? IsDelivered { get; set; }
        public string DeliveredAt { get; set; }

        public virtual Payments Payment { get; set; }
        public virtual Users User { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
