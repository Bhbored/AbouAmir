using AbouAmir.Convertors;
using AbouAmir.MVVM.Models;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbouAmir.Services
{
    public class FirestoreService
    {
        private FirestoreDb? db;

        private async Task SetupFirestore()
        {
            if (db == null)
            {
                var stream = await FileSystem.OpenAppPackageFileAsync("abou-amir-firebase-adminsdk-fbsvc-0bf2a65b57.json");
                using var reader = new StreamReader(stream);
                var contents = reader.ReadToEnd();

                db = new FirestoreDbBuilder
                {
                    ProjectId = "abou-amir",
                    JsonCredentials = contents,
                    ConverterRegistry = new ConverterRegistry
                    {
                        new DateTimeToTimestampConverter(),
                    }
                }.Build();
            }
        }

        // Add a single product
        public async Task InsertProduct(Product product)
        {
            await SetupFirestore();
            await db.Collection("products").AddAsync(product);
        }

        // Add a list of products
        public async Task InsertProducts(List<Product> productList)
        {
            await SetupFirestore();
            foreach (var product in productList)
            {
                await db.Collection("products").AddAsync(product);
            }
        }

        // Retrieve all products
        public async Task<List<Product>> GetProducts()
        {
            await SetupFirestore();
            var snapshot = await db.Collection("products").GetSnapshotAsync();
            return snapshot.Documents.Select(doc =>
            {
                var product = doc.ConvertTo<Product>();
                product.Id = doc.Id;
                return product;
            }).ToList();
        }

        // Update a product by ID
        public async Task UpdateProduct(Product product)
        {
            if (string.IsNullOrEmpty(product.Id)) return;
            await SetupFirestore();
            var docRef = db.Collection("products").Document(product.Id);
            await docRef.SetAsync(product, SetOptions.Overwrite);
        }

        // Delete a product by ID
        public async Task DeleteProduct(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            await SetupFirestore();
            var docRef = db.Collection("products").Document(id);
            await docRef.DeleteAsync();
        }
    }
}
