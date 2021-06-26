using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MyBot.Models
{
    public partial class CustomerSentiment
    {
        public int Id { get; set; }
        public TimeSpan? Time { get; set; }
        public string VegaPredict { get; set; }
        public string VegaComment { get; set; }
        public string FoodPredict { get; set; }
        public string FoodComment { get; set; }
        public string ServicePredict { get; set; }
        public string ServiceComment { get; set; }
        public string NameByUser { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
