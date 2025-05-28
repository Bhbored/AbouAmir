using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbouAmir.MVVM.Models
{
    public class Product
    {
        public string? Name { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
        public DateTime DateAdded { get; set; }
        
    }
}
