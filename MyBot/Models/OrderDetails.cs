using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MyBot.Models
{
    public partial class OrderDetails
    {
        public int OrderId { get; set; }
        public int MenuId { get; set; }
        public int? Quantity { get; set; }

        public virtual Menus Menu { get; set; }
        public virtual Orders Order { get; set; }
    }
}
