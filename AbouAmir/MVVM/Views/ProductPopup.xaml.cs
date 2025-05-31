using AbouAmir.MVVM.Models;
using AbouAmir.MVVM.ViewModels;
using AbouAmir.Services;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;

namespace AbouAmir.Views
{
    [AddINotifyPropertyChangedInterface]
    public partial class ProductPopup : Popup
    {
        private readonly AppDbContext _db = AppDbContext.Instance;
        private readonly IViewModel _viewModel = IViewModel.Instance;

        public ProductPopup()
        {
            InitializeComponent();
            BindingContext = this;
           
        }

        private string? _imagePath;


        public List<string> categoryNames { get; set; } = ["Snacks", "Dairy", "Bakery", "Beverages"];
        

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null)
                    return;

                var localPath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using var stream = await photo.OpenReadAsync();
                using var fileStream = File.OpenWrite(localPath);
                await stream.CopyToAsync(fileStream);

                PreviewImage.Source = ImageSource.FromFile(localPath);
                _imagePath = localPath;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Camera Error", $"Unable to capture photo: {ex.Message}", "OK");
            }
        }

        private async void OnOkClicked(object sender, EventArgs e)
        {
            // Get user inputs
            string? name = NameEntry?.Text;
            string? priceText = PriceEntry?.Text;
            string? stockText = StockEntry?.Text;
            string? category = picker.SelectedItem?.ToString();      

            // Validation
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(priceText) ||
                string.IsNullOrWhiteSpace(stockText) ||
                string.IsNullOrWhiteSpace(_imagePath) ||
                string.IsNullOrWhiteSpace(category))
            {
                await Application.Current.MainPage.DisplayAlert("Missing Fields", "Please fill all fields and take a photo.", "OK");
                return;
            }

            if (!double.TryParse(priceText, out double price))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Input", "Price must be a valid number.", "OK");
                return;
            }

            if (!int.TryParse(stockText, out int stock))
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Input", "Stock must be a valid integer.", "OK");
                return;
            }

            // Create new product with unique Id
            var product = new Product
            {
                Id = Guid.NewGuid().ToString(),
                Name = name,
                Price = price,
                Stock = stock,
                ImageUrl = _imagePath,
                Category = category
            };
           

            try
            {
                // Add product to DB
                await _db.InsertProductAsync(product);
                await Application.Current.MainPage.DisplayAlert("Success", "Product added successfully.", "OK");

                Close();
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to add product: {ex.Message}", "OK");
            }
        }


        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close();
        }
     

    }
}
