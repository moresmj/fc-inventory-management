using InventorySystemAPI.Models.Company;
using InventorySystemAPI.Models.Item;
using InventorySystemAPI.Models.Item.Attribute;
using InventorySystemAPI.Models.Item.Category;
using InventorySystemAPI.Models.Size;
using InventorySystemAPI.Models.Store;
using InventorySystemAPI.Models.Store.Inventory;
using InventorySystemAPI.Models.Store.Releasing;
using InventorySystemAPI.Models.Store.Request;
using InventorySystemAPI.Models.Store.Sales;
using InventorySystemAPI.Models.Store.Stock;
using InventorySystemAPI.Models.User;
using InventorySystemAPI.Models.Warehouse;
using InventorySystemAPI.Models.Warehouse.Inventory;
using InventorySystemAPI.Models.Warehouse.Stock;
using Microsoft.EntityFrameworkCore;

namespace InventorySystemAPI.Context
{
    public class FloorCenterContext : DbContext
    {

        public FloorCenterContext(DbContextOptions<FloorCenterContext> options)
            : base(options)
        { }

        public DbSet<Item> Items { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Warehouse> Warehouses { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Size> Sizes { get; set; }

        public DbSet<WHInventory> WHInventories { get; set; }

        public DbSet<WHInventoryDetail> WHInventoryDetails { get; set; }

        public DbSet<WHStock> WHStocks { get; set; }

        public DbSet<STInventory> STInventories { get; set; }

        public DbSet<STInventoryDetail> STInventoryDetails { get; set; }

        public DbSet<STStock> STStocks { get; set; }

        public DbSet<STDelivery> STDeliveries { get; set; }

        public DbSet<STDeliveryDetail> STDeliveryDetails { get; set; }

        public DbSet<STSales> STSales { get; set; }

        public DbSet<STSalesDetail> STSalesDetails { get; set; }

        public DbSet<STRelease> STReleases { get; set; }

        public DbSet<STReleaseDetail> STReleaseDetails { get; set; }

        public DbSet<CategoryParent> CategoryParents { get; set; }

        public DbSet<CategoryChild> CategoryChildren { get; set; }

        public DbSet<CategoryGrandChild> CategoryGrandChildren { get; set; }

        public DbSet<ItemAttribute> ItemAttributes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().ToTable("Item");
            modelBuilder.Entity<Store>().ToTable("Store");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Warehouse>().ToTable("Warehouse");
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Size>().ToTable("Size");
            modelBuilder.Entity<WHInventory>().ToTable("WHInventory");
            modelBuilder.Entity<WHInventoryDetail>().ToTable("WHInventoryDetail");
            modelBuilder.Entity<WHStock>().ToTable("WHStock");
            modelBuilder.Entity<STInventory>().ToTable("STInventory");
            modelBuilder.Entity<STInventoryDetail>().ToTable("STInventoryDetail");
            modelBuilder.Entity<STStock>().ToTable("STStock");
            modelBuilder.Entity<STDelivery>().ToTable("STDelivery");
            modelBuilder.Entity<STDeliveryDetail>().ToTable("STDeliveryDetail");
            modelBuilder.Entity<STSales>().ToTable("STSales");
            modelBuilder.Entity<STSalesDetail>().ToTable("STSalesDetail");
            modelBuilder.Entity<STRelease>().ToTable("STRelease");
            modelBuilder.Entity<STReleaseDetail>().ToTable("STReleaseDetail");
            modelBuilder.Entity<CategoryParent>().ToTable("CategoryParent");
            modelBuilder.Entity<CategoryChild>().ToTable("CategoryChild");
            modelBuilder.Entity<CategoryGrandChild>().ToTable("CategoryGrandChild");
            modelBuilder.Entity<ItemAttribute>().ToTable("ItemAttribute");
        }

    }
}
