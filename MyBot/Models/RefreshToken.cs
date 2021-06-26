﻿using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MyBot.Models
{
    public partial class RefreshToken
    {
        public int TokenId { get; set; }
        public int? CustomerId { get; set; }
        public string Token { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public virtual Users Customer { get; set; }
    }
}
