using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MyBot.Models
{
    public partial class Products
    {
        public Products()
        {
            MenusDetail = new HashSet<MenusDetail>();
        }

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal? Price { get; set; }
        public string Description { get; set; }
        public int? Calories { get; set; }
        public float? Protein { get; set; }
        public float? Carb { get; set; }
        public float? Fat { get; set; }

        public virtual ICollection<MenusDetail> MenusDetail { get; set; }
    }
}
