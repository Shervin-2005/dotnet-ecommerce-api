using Domain.Entities;
using Domain.Entities.Menu;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ProductSpecification> ProductSpecifications { get; set; } = null!;
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<OtpVerification> OtpVerifications { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<SupportConversation> SupportConversations { get; set; } = null!;
        public DbSet<SupportMessage> SupportMessages{ get; set; }  = null!;
        public DbSet<FlashSale> FlashSales { get; set; } = null!;
        public DbSet<FlashSaleItem> FlashSaleItems { get; set; } = null!;
        public DbSet<Menu> Menus { get; set; } = null!;
        public DbSet<MenuItem> MenuItems { get; set; } = null!;
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations automatically
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => !u.IsDeleted);
        }
    }
}