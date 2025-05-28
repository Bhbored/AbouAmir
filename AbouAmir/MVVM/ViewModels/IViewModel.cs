using AbouAmir.MVVM.Models;
using AbouAmir.Services;
using AbouAmir.Views;
using CommunityToolkit.Maui.Views;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Reflection.Metadata.BlobBuilder;

namespace AbouAmir.MVVM.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class IViewModel
    {
        private static IViewModel? _instance;

        public static IViewModel Instance => _instance ??= new IViewModel();

        private readonly FirestoreService _firestoreService = new();
        
        public IViewModel()
        {
            _ = LoadProductsFromFirebaseAsync(); // Load on startup
            filteredProducts = new ObservableCollection<Product>(Products.Where(x => x.Category == "Snacks"));
        }

        public ObservableCollection<Category> Categories { get; set; } = [
            new() { Name = "Snacks", ImageUrl = "cookie.gif" },
            new(){ Name = "Dairy", ImageUrl = "milk.gif" },
            new(){ Name = "Bakery", ImageUrl = "bread.gif" },
            new(){ Name = "Beverages", ImageUrl = "frappe.gif"},
            ];
      

        public ObservableCollection<Product> Products { get; set; } = [];
        public ObservableCollection<Product> filteredProducts { get; set; } = [];
        



        #region Commands

        public async Task LoadProductsFromFirebaseAsync()
        {
            var productsFromFirebase = await _firestoreService.GetProducts();

            Products.Clear();
            foreach (var product in productsFromFirebase)
            {
                Products.Add(product);
            }
        }

        public ICommand Choose => new Command<string>(categoryName =>
        {
            if (string.IsNullOrEmpty(categoryName)) return;

            var category = Categories?.FirstOrDefault(c => c.Name == categoryName);
            if (category == null) return;

            filteredProducts.Clear();

            foreach (var product in Products.Where(x => x.Category == category.Name))
            {
                filteredProducts.Add(product);
            }
        });

        #endregion


    }

}

