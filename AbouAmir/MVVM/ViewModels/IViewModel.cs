using AbouAmir.MVVM.Models;
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
        public IViewModel()
        {   
            foreach(var product in Dairy) CurrentInventory.Add(product);
            foreach (var product in Snacks) CurrentInventory.Add(product);
            foreach (var product in Bakery) CurrentInventory.Add(product);
            foreach (var product in Beverages) CurrentInventory.Add(product);
            Products = new ObservableCollection<Product>(Snacks);
        }

        public ObservableCollection<Category> Categories { get; set; } = [
            new() { Name = "Snacks", ImageUrl = "cookie.gif" },
            new(){ Name = "Dairy", ImageUrl = "milk.gif" },
            new(){ Name = "Bakery", ImageUrl = "bread.gif" },
            new(){ Name = "Beverages", ImageUrl = "frappe.gif"},
            ];
        public ObservableCollection<Product> CurrentInventory { get; set; } = [];

        public ObservableCollection<Product> Products { get; set; } = [];


        #region Product Collections

        public ObservableCollection<Product> Snacks { get; set; } = [
           new Product { Name = "Chocolate Bar", Price = 2.50, Stock = 30, ImageUrl = "milk.gif", Category = "Snacks", DateAdded = DateTime.Now },

            ];
        public ObservableCollection<Product> Dairy { get; set; } = [
              new Product { Name = "Milk", Price = 3.00, Stock = 20, ImageUrl = "", Category = "Dairy", DateAdded = DateTime.Now },
             new Product { Name = "Butter", Price = 5.50, Stock = 12, ImageUrl = "", Category = "Dairy", DateAdded = DateTime.Now },
            ];

        public ObservableCollection<Product> Bakery { get; set; } =
            [new Product { Name = "Bread Loaf", Price = 1.80, Stock = 15, ImageUrl = "", Category = "Bakery", DateAdded = DateTime.Now },
        ];
        public ObservableCollection<Product> Beverages { get; set; } = [
            new Product { Name = "Coffee", Price = 2.00, Stock = 25, ImageUrl = "", Category = "Beverages", DateAdded = DateTime.Now },
            new Product { Name = "Tea", Price = 1.50, Stock = 18, ImageUrl = "", Category = "Beverages", DateAdded = DateTime.Now }
        ];

        #endregion


        #region Commands
        public ICommand Choose => new Command<string>(categoryName =>
        {
            if (string.IsNullOrEmpty(categoryName)) return;

            var category = Categories?.FirstOrDefault(c => c.Name == categoryName);
            if (category == null) return;

            ObservableCollection<Product> filteredProducts = [];
            switch(category.Name)
            {
                case "Snacks":
                    filteredProducts = Snacks;
                    break;
                case "Dairy":
                    filteredProducts = Dairy;
                    break;
                case "Bakery":
                    filteredProducts = Bakery;
                    break;
                case "Beverages":
                    filteredProducts = Beverages;
                    break;
            }

            Products.Clear();
            foreach (var product in filteredProducts)
            {
                Products.Add(product);
            }
        });

       
    }
    #endregion


}

