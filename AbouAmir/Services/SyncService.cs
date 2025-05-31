using AbouAmir.MVVM.Models;
using System.Diagnostics;

namespace AbouAmir.Services
{
    public class SyncService
    {
        private readonly AppDbContext _localDb = AppDbContext.Instance;
        private readonly FirestoreService _firebase = FirestoreService.Instance;
        public static SyncService Instance => _instance ??= new SyncService();
        private static SyncService? _instance;

        public SyncService() { }

        public async Task SyncFromLocalToFirebase()
        {
            try
            {
                var localProducts = await _localDb.GetProductsAsync();
                var firebaseProducts = await _firebase.GetProducts();

                var localDict = localProducts.ToDictionary(p => p.Name.Trim().ToLower(), p => p);
                var firebaseDict = firebaseProducts.ToDictionary(p => p.Name.Trim().ToLower(), p => p);

                // Upload or update local products to Firebase
                foreach (var localProduct in localDict.Values)
                {
                    if (!firebaseDict.ContainsKey(localProduct.Name.Trim().ToLower()))
                    {
                        await _firebase.InsertProduct(localProduct);
                        Debug.WriteLine($"✅ Inserted to Firebase: {localProduct.Name}");
                    }
                    else
                    {
                        await _firebase.UpdateProduct(localProduct);
                        Debug.WriteLine($"🔄 Updated in Firebase: {localProduct.Name}");
                    }
                }

                // Delete Firebase products not present in local DB
                foreach (var firebaseProduct in firebaseDict.Values)
                {
                    if (!localDict.ContainsKey(firebaseProduct.Name.Trim().ToLower()))
                    {
                        await _firebase.DeleteProduct(firebaseProduct);
                        Debug.WriteLine($"🗑️ Deleted from Firebase: {firebaseProduct.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Sync ERROR] {ex.Message}");
            }
        }
    }
}
