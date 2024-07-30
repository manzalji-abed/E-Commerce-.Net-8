using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MvcBul.Models;

namespace MvcBul.DataAccess.Data
{
    public class ApplicationDbContext: IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
                new Category { Id = 3, Name = "History", DisplayOrder = 3 }
                );

            modelBuilder.Entity<Product>().HasData(
                    new Product
                    {
                        Id = 1,
                        Title = "Book 1",
                        Description = "Description for Book 1",
                        ISBN = "9781234567890",
                        Author = "Author A",
                        ListPrice = 29.99,
                        Price = 19.99,
                        Price50 = 17.99,
                        Price100 = 15.99,
                        CategoryId = 1,
                    },
                    new Product
                    {
                        Id = 2,
                        Title = "Book 2",
                        Description = "Description for Book 2",
                        ISBN = "9780987654321",
                        Author = "Author B",
                        ListPrice = 24.99,
                        Price = 14.99,
                        Price50 = 12.99,
                        Price100 = 10.99,
                        CategoryId = 1,
                    },
                    new Product
                    {
                        Id = 3,
                        Title = "Book 3",
                        Description = "Description for Book 3",
                        ISBN = "9785555555555",
                        Author = "Author C",
                        ListPrice = 19.99,
                        Price = 9.99,
                        Price50 = 8.99,
                        Price100 = 7.99,
                        CategoryId = 3,
                    },
                    new Product
                    {
                        Id = 4,
                        Title = "Book 4",
                        Description = "Description for Book 4",
                        ISBN = "9786666666666",
                        Author = "Author D",
                        ListPrice = 34.99,
                        Price = 24.99,
                        Price50 = 22.99,
                        Price100 = 19.99,
                        CategoryId = 2,
                    },
                    new Product
                    {
                        Id = 5,
                        Title = "Book 5",
                        Description = "Description for Book 5",
                        ISBN = "9787777777777",
                        Author = "Author E",
                        ListPrice = 27.99,
                        Price = 17.99,
                        Price50 = 15.99,
                        Price100 = 12.99,
                        CategoryId = 2,
                    },
                    new Product
                    {
                        Id = 6,
                        Title = "Book 6",
                        Description = "Description for Book 6",
                        ISBN = "9788888888888",
                        Author = "Author F",
                        ListPrice = 21.99,
                        Price = 11.99,
                        Price50 = 9.99,
                        Price100 = 7.99,
                        CategoryId = 2,
                        
                    },
                    new Product
                    {
                        Id = 7,
                        Title = "Book 7",
                        Description = "Description for Book 7",
                        ISBN = "9789999999999",
                        Author = "Author G",
                        ListPrice = 31.99,
                        Price = 21.99,
                        Price50 = 19.99,
                        Price100 = 16.99,
                        CategoryId = 1,                   
                    },
                    new Product
                    {
                        Id = 8,
                        Title = "Book 8",
                        Description = "Description for Book 8",
                        ISBN = "9781010101010",
                        Author = "Author H",
                        ListPrice = 23.99,
                        Price = 13.99,
                        Price50 = 11.99,
                        Price100 = 9.99,
                        CategoryId = 3,
                       
                    },
                    new Product
                    {
                        Id = 9,
                        Title = "Book 9",
                        Description = "Description for Book 9",
                        ISBN = "9781111111111",
                        Author = "Author I",
                        ListPrice = 18.99,
                        Price = 8.99,
                        Price50 = 7.99,
                        Price100 = 6.99,
                        CategoryId = 1,
                     
                    },
                    new Product
                    {
                        Id = 10,
                        Title = "Book 10",
                        Description = "Description for Book 10",
                        ISBN = "9781212121212",
                        Author = "Author J",
                        ListPrice = 26.99,
                        Price = 16.99,
                        Price50 = 14.99,
                        Price100 = 11.99,
                        CategoryId = 2,
                    }
                );
        }
    }
}
