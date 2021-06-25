using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyBot
{
    public class User
    {
        public List<string> TasksList = new List<string>();
        public string Name { get; set; }
        public string Sex { get; set; }
        public int  Age { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public double BMI { get; set; }
        public string Diseases { get; set; }
    }
}
