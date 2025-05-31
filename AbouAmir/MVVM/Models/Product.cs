using Google.Cloud.Firestore;
using PropertyChanged;
using System;
using System.ComponentModel.DataAnnotations;

namespace AbouAmir.MVVM.Models
{
    [AddINotifyPropertyChangedInterface]
    [FirestoreData]
    public class Product
    {
        [Key]
        [FirestoreDocumentId] // ✅ This tells Firestore to map the doc ID, not store it
        public string? Id { get; set; }

        [FirestoreProperty]
        public string? Name { get; set; }

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
