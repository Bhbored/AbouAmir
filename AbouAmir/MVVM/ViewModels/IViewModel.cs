using AbouAmir.MVVM.Models;
using AbouAmir.Services;
using AbouAmir.Views;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
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
        private readonly AppDbContext dataBase = AppDbContext.Instance;
        private readonly SyncService syncService = SyncService.Instance;
        public string CurrentCategoryFilter { get; set; } = "Snacks";  // default initial filter


        public IViewModel()
        {
            _ = LoadProductsFromDB();

            filteredProducts.Clear();
            foreach (var product in Products.Where(x => x.Category == "Snacks"))
                filteredProducts.Add(product);
        }

           
        

        public ObservableCollection<Category> Categories { get; set; } = [
            new() { Name = "Snacks", ImageUrl = "cookie.gif" },
            new(){ Name = "Dairy", ImageUrl = "milk.gif" },
            new(){ Name = "Bakery", ImageUrl = "bread.gif" },
            new(){ Name = "Beverages", ImageUrl = "frappe.gif"},
            ];

        #region properties

        public ObservableCollection<Product> Products { get; set; } = [];
        public ObservableCollection<Product> filteredProducts { get; set; } = [];

        
        public bool IsRefreshing { get; set; }



        #endregion



        #region Commands

        public async Task LoadProductsFromDB()
        {
            var productsFromDB = await dataBase.GetProductsAsync();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Products.Clear();
                filteredProducts.Clear();

                foreach (var product in productsFromDB)
                {
                    Products.Add(product);
                }

                // Also update filteredProducts for initial filter
                var currentCategory = CurrentCategoryFilter ?? "Snacks";
                foreach (var product in Products.Where(x => x.Category == currentCategory))
                {
                    filteredProducts.Add(product);
                }
            });
        }


        public ICommand Choose => new Command<string>(categoryName =>
        {
            if (string.IsNullOrEmpty(categoryName)) return;

            CurrentCategoryFilter = categoryName;

            filteredProducts.Clear();

            foreach (var product in Products.Where(x => x.Category == categoryName))
            {
                filteredProducts.Add(product);
            }
        });


        public ICommand RefreshCommand => new Command(async() => { await RefreshAsync(); });
        public ICommand Delete => new Command<string>(async (productName) =>
        {
            if (string.IsNullOrEmpty(productName)) return;

            var productToDelete = Products.FirstOrDefault(p => p.Name == productName);
            if (productToDelete == null) return;

            bool confirmDelete = await Application.Current.MainPage.DisplayAlert(
                "⚠️ Delete Product?",
                $"Are you sure you want to remove {productToDelete.Name}? This action cannot be undone.",
                "Delete", "Cancel");

            if (!confirmDelete) return;

            //Remove product from collection & database
            await dataBase.DeleteProductAsync(productToDelete);
            filteredProducts.Remove(productToDelete);

            // Snackbar with Undo option
            var snackbarOptions = new SnackbarOptions
            {
                BackgroundColor = Colors.Black,
                TextColor = Colors.White,
                ActionButtonTextColor = Colors.Green,
                CornerRadius = 10,
            };

            var snackbar = Snackbar.Make(
                $"✅ {productToDelete.Name} deleted",
                async () =>
                {
                    filteredProducts.Add(productToDelete); // Restore on undo
                    await dataBase.InsertProductAsync(productToDelete); // Restore on undo
                    await Toast.Make("🛠️ Product restored!").Show();
                },
                "Undo",
                TimeSpan.FromSeconds(4),
                snackbarOptions);

            await snackbar.Show();
        });

        public async Task RefreshAsync()
        {
            try
            {
                IsRefreshing = true;
                await syncService.SyncFromLocalToFirebase();
                //await dataBase.DeleteAllProductsAsync(); //just for testing purposes
            }
            catch (Exception ex)
            {
                // Log or display error message
                Console.WriteLine($"[Sync ERROR] {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }


        #endregion


    }

}

