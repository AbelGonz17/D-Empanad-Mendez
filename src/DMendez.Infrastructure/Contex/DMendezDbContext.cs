using DMendez.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DMendez.Infrastructure.Contex
{
    public class DMendezDbContext : IdentityDbContext
    {
        public DMendezDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ProductInventory>()
                .HasKey(pi => pi.ProductId);

            builder.Entity<Order>()
                .Property(o => o.DeliveryFee)
                .HasPrecision(18, 2);

            builder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);
                
            builder.Entity<Combo>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<ProductDiscount>()
                .Property(pd => pd.Percentage)
                .HasPrecision(5, 2);
                
            builder.Entity<DeliveryZone>()
                .Property(dz => dz.DeliveryFee)
                .HasPrecision(18, 2);
        }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<BusinessClosure> BusinessClosures { get; set; }
        public DbSet<BusinessHours> BusinessHours { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboItem> ComboItems { get; set; }
        public DbSet<DeliveryZone> DeliveryZones { get; set; }
        public DbSet<InventoryReservation> InventoryReservations { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDiscount> ProductDiscounts { get; set; }
        public DbSet<ProductInventory> ProductInventories { get; set; }
    }
}