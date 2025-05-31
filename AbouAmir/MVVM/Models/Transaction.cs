using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbouAmir.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    public class Transaction
    {

        public string? Id { get; set; } 
        public String? ProuctName { get; set; }
        public double Price { get; set; }
        public int Stock { get; set; }
        public string? Category { get; set; }
        public DateTime Date { get; set; }
        public string? Type { get; set; } 
    }
}
