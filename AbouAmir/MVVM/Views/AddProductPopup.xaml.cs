using AbouAmir.MVVM.Models;
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace AbouAmir.Views
{
    public partial class ProductPopup : Popup
    {
        private string? imagePath;

        public ObservableCollection<Product> ProductList { get; set; }

        public ProductPopup(ObservableCollection<Product> productList)
        {
            InitializeComponent();
            ProductList = productList;
        }

        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                var photo = await MediaPicker.CapturePhotoAsync();
                if (photo == null) return;

                var fileName = $"product_{DateTime.Now:yyyyMMddHHmmss}.jpg";
                imagePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

                using var stream = await photo.OpenReadAsync();
                using var newStream = File.OpenWrite(imagePath);
                await stream.CopyToAsync(newStream);

                PreviewImage.Source = imagePath;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private async void OnOkClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameEntry.Text) ||
                !double.TryParse(PriceEntry.Text, out double price) ||
                !int.TryParse(StockEntry.Text, out int stock))
            {
                await Shell.Current.DisplayAlert("Invalid", "Fill all fields correctly", "OK");
                return;
            }

            var product = new Product
            {
                Name = NameEntry.Text,
                Price = price,
                Stock = stock,
                ImageUrl = imagePath,
                DateAdded = DateTime.Now
            };

            ProductList.Add(product); // directly add to the shared list
            Close();
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            Close(); // do nothing
        }
    }
}
