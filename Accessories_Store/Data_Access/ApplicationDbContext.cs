using Microsoft.EntityFrameworkCore;
using Accessories_Store.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Accessories_Store.Models.Entities;

namespace Accessories_Store.Data_Access
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Blog> Blogs { get; set; }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategoryType> CategoryTypes { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }

        public virtual DbSet<Contact> Contacts { get; set; }

        public virtual DbSet<Keyword> Keywords { get; set; }

        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<ProductMaterial> ProductMaterials { get; set; }


        public virtual DbSet<OrderDetail> OrderDetails { get; set; }

        public virtual DbSet<Product> Products { get; set; }

        public virtual DbSet<ProductImage> ProductImages { get; set; }

        public virtual DbSet<ProductVariant> ProductVariants { get; set; }

        public virtual DbSet<Rating> Ratings { get; set; }

        public virtual DbSet<Slide> Slides { get; set; }

        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<PaymentCoupon> PaymentCoupons { get; set; }


    }
}
