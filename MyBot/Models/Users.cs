using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MyBot.Models
{
    public partial class Users
    {
        public Users()
        {
            Orders = new HashSet<Orders>();
            RefreshToken = new HashSet<RefreshToken>();
        }

        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool? IsAdmin { get; set; }
        public string PhoneNumber { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<RefreshToken> RefreshToken { get; set; }
    }
}
