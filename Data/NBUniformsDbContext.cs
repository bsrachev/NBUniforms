﻿namespace NBUniforms.Data
{
    using NBUniforms.Data.Models;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class NBUniformsDbContext : IdentityDbContext<User>
    {
        public NBUniformsDbContext(DbContextOptions<NBUniformsDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; init; }

        public DbSet<Category> Categories { get; init; }

        public DbSet<ProductSizeQuantity> ProductSizeQuantities { get; init; }

        public DbSet<Cart> Carts { get; init; }

        public DbSet<CartProduct> CartProducts { get; init; }

        public DbSet<Order> Orders { get; init; }
                    
        public DbSet<OrderProduct> OrderProducts { get; init; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder
                .Entity<Product>()
                .HasOne(c => c.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<ProductSizeQuantity>()
                .HasOne(p => p.Product)
                .WithMany(p => p.ProductSizeQuantities)
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .Entity<Cart>()
                .HasOne(u => u.User)
                .WithOne(c => c.Cart)
                .HasForeignKey<User>(c => c.CartId)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(builder);
        }
    }
}
