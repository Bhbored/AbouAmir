using AbouAmir.MVVM.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AbouAmir.Services
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        private static AppDbContext? _instance;
        public static AppDbContext Instance => _instance ??= new AppDbContext();

        private string _dbPath;

        public AppDbContext()
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            _dbPath = Path.Combine(folder, "AbouAmir.db");
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_dbPath}");
        }

        #region Product Methods

        // Get all products
        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await Products.ToListAsync();
            foreach (var product in products)
            {
                Debug.WriteLine($"📦 Product: {product.Name}");
            }
            return products;
        }

        // Insert a product (avoid duplicates)
        public async Task InsertProductAsync(Product product)
        {
            var existing = await Products.FirstOrDefaultAsync(p => p.Name == product.Name);
            if (existing != null)
            {
                Debug.WriteLine($"⚠️ Duplicate not inserted: {product.Name}");
                return;
            }

            await Products.AddAsync(product);
            await SaveChangesAsync();
            Debug.WriteLine($"✅ Inserted product: {product.Name}");
        }

        // Update product
        public async Task UpdateProductAsync(Product product)
        {
            Products.Update(product);
            await SaveChangesAsync();
            Debug.WriteLine($"🔄 Updated product: {product.Name}");
        }

        // Delete product (safely with tracked entity)
        public async Task DeleteProductAsync(Product product)
        {
            var existing = await Products.FirstOrDefaultAsync(p => p.Name == product.Name);
            if (existing == null)
            {
                Debug.WriteLine($"⚠️ Cannot delete. Product not found: {product.Name}");
                return;
            }

            Products.Remove(existing);
            await SaveChangesAsync();
            Debug.WriteLine($"🗑️ Deleted product: {product.Name}");
        }

        // Delete all products
        public async Task DeleteAllProductsAsync()
        {
            var allProducts = await Products.ToListAsync();
            Products.RemoveRange(allProducts);
            await SaveChangesAsync();
            Debug.WriteLine("🧹 All products deleted from local database.");
        }

        #endregion

        #region Transaction Methods



        #endregion
    }
}
