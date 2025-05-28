using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbouAmir.MVVM.Models
{
    [FirestoreData]
    public class Product
    {
        [FirestoreProperty]
        public string? Name { get; set; }
        [FirestoreProperty]
        public string? Id { get; set; }
        [FirestoreProperty]
        public double Price { get; set; }
        [FirestoreProperty]
        public int Stock { get; set; }
        [FirestoreProperty]
        public string? ImageUrl { get; set; }
        [FirestoreProperty]
        public string? Category { get; set; }
        [FirestoreProperty]
        public DateTime DateAdded { get; set; }
        
    }
}
