using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MyBot.Models
{
    public partial class Menus
    {
        public Menus()
        {
            MenusDetail = new HashSet<MenusDetail>();
            OrderDetails = new HashSet<OrderDetails>();
        }

        public int MenuId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }

        public virtual ICollection<MenusDetail> MenusDetail { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }
    }
}
