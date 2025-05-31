using AbouAmir.Convertors;
using AbouAmir.MVVM.Models;
using Google.Cloud.Firestore;
using System.Diagnostics;
using System.IO;

namespace AbouAmir.Services
{
    public class FirestoreService
    {
        private FirestoreDb? db;
        public static FirestoreService Instance => _db ??= new FirestoreService();
        private static FirestoreService? _db;

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

        // Helper: Get document ID by product name
        private async Task<string?> GetDocumentIdByName(string name)
        {
            var snapshot = await db.Collection("products").GetSnapshotAsync();
            var doc = snapshot.Documents.FirstOrDefault(d =>
            {
                var p = d.ConvertTo<Product>();
                return p.Name?.Trim().ToLower() == name.Trim().ToLower();
            });

            return doc?.Id;
        }

        public async Task InsertProduct(Product product)
        {
            await SetupFirestore();

            var existingId = await GetDocumentIdByName(product.Name);
            if (existingId != null)
            {
                Debug.WriteLine($"⚠️ Firebase insert skipped: Product \"{product.Name}\" already exists.");
                return;
            }

            var docRef = await db.Collection("products").AddAsync(product);
            product.Id = docRef.Id;

            Debug.WriteLine($"✅ Firebase insert: Product \"{product.Name}\" added (ID: {docRef.Id}).");
        }

        public async Task InsertProducts(List<Product> productList)
        {
            await SetupFirestore();
            foreach (var product in productList)
            {
                var existingId = await GetDocumentIdByName(product.Name);
                if (existingId != null)
                {
                    Debug.WriteLine($"⚠️ Firebase insert skipped: Product \"{product.Name}\" already exists.");
                    continue;
                }

                var docRef = await db.Collection("products").AddAsync(product);
                product.Id = docRef.Id;

                Debug.WriteLine($"✅ Firebase insert: Product \"{product.Name}\" added.");
            }
        }

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

        public async Task UpdateProduct(Product product)
        {
            await SetupFirestore();

            var docId = await GetDocumentIdByName(product.Name);
            if (docId == null)
            {
                Debug.WriteLine($"⚠️ Update skipped: No product found with name \"{product.Name}\"");
                return;
            }

            var docRef = db.Collection("products").Document(docId);
            await docRef.SetAsync(product, SetOptions.Overwrite);

            Debug.WriteLine($"🔄 Firebase update: Product \"{product.Name}\" updated.");
        }

        public async Task DeleteProduct(Product product)
        {
            await SetupFirestore();

            var docId = await GetDocumentIdByName(product.Name);
            if (docId == null)
            {
                Debug.WriteLine($"⚠️ Delete skipped: No product found with name \"{product.Name}\"");
                return;
            }

            var docRef = db.Collection("products").Document(docId);
            await docRef.DeleteAsync();

            Debug.WriteLine($"🗑️ Firebase delete: Product \"{product.Name}\" deleted.");
        }
    }
}
