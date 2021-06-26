using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace MyBot.Models
{
    public partial class VegafoodBotContext : DbContext
    {
        public VegafoodBotContext()
        {
        }

        public VegafoodBotContext(DbContextOptions<VegafoodBotContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AppRole> AppRole { get; set; }
        public virtual DbSet<CustomerSentiment> CustomerSentiment { get; set; }
        public virtual DbSet<Menus> Menus { get; set; }
        public virtual DbSet<MenusDetail> MenusDetail { get; set; }
        public virtual DbSet<OrderDetails> OrderDetails { get; set; }
        public virtual DbSet<Orders> Orders { get; set; }
        public virtual DbSet<Payments> Payments { get; set; }
        public virtual DbSet<Products> Products { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Data Source=DESKTOP-N30SPMS;Initial Catalog=VegafoodBot;User ID=sa;Password=1";
            if (!optionsBuilder.IsConfigured)
            {
                // optionsBuilder.UseSqlServer("Name=Vegafood");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppRole>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(50);

                entity.Property(e => e.Name).HasMaxLength(30);
            });

            modelBuilder.Entity<CustomerSentiment>(entity =>
            {
                entity.Property(e => e.CustomerName).HasMaxLength(30);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FoodComment).HasMaxLength(30);

                entity.Property(e => e.FoodPredict)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.NameByUser).HasMaxLength(30);

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.Property(e => e.ServiceComment).HasMaxLength(30);

                entity.Property(e => e.ServicePredict)
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.VegaComment).HasMaxLength(30);

                entity.Property(e => e.VegaPredict)
                    .HasMaxLength(10)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Menus>(entity =>
            {
                entity.HasKey(e => e.MenuId)
                    .HasName("PK__Menus__3B407174395EE571");

                entity.Property(e => e.MenuId).HasColumnName("menuId");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(50);

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasColumnType("ntext");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<MenusDetail>(entity =>
            {
                entity.HasKey(e => new { e.MenuId, e.ProductId })
                    .HasName("menusdetail_pk");

                entity.Property(e => e.MenuId).HasColumnName("menuId");

                entity.Property(e => e.ProductId).HasColumnName("productId");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.MenusDetail)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MenusDetail_Menus");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.MenusDetail)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MenusDetail_Products");
            });

            modelBuilder.Entity<OrderDetails>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.MenuId })
                    .HasName("orderdetail_pk");

                entity.Property(e => e.OrderId).HasColumnName("orderId");

                entity.Property(e => e.MenuId).HasColumnName("menuId");

                entity.HasOne(d => d.Menu)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.MenuId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Menus");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderDetails_Orders");
            });

            modelBuilder.Entity<Orders>(entity =>
            {
                entity.HasKey(e => e.OrderId)
                    .HasName("PK__Orders__0809335D0B35B1F6");

                entity.Property(e => e.OrderId).HasColumnName("orderId");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(50);

                entity.Property(e => e.DeliveredAt)
                    .HasColumnName("deliveredAt")
                    .HasMaxLength(30)
                    .IsFixedLength();

                entity.Property(e => e.IsDelivered).HasColumnName("isDelivered");

                entity.Property(e => e.IsPaid).HasColumnName("isPaid");

                entity.Property(e => e.OrderStatus).HasColumnName("orderStatus");

                entity.Property(e => e.PaidAt)
                    .HasColumnName("paidAt")
                    .HasMaxLength(30)
                    .IsFixedLength();

                entity.Property(e => e.PaymentId).HasColumnName("paymentId");

                entity.Property(e => e.PaypalMethod)
                    .HasColumnName("paypalMethod")
                    .HasMaxLength(30);

                entity.Property(e => e.ShippingPrice).HasColumnName("shippingPrice");

                entity.Property(e => e.TotalPrice).HasColumnName("totalPrice");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Payment)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.PaymentId)
                    .HasConstraintName("FK_Orders_Payments");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Orders_Users");
            });

            modelBuilder.Entity<Payments>(entity =>
            {
                entity.HasKey(e => e.PaymentId)
                    .HasName("PK__Payments__A0D9EFC60F90176A");

                entity.Property(e => e.PaymentId).HasColumnName("paymentId");

                entity.Property(e => e.EmailAddress)
                    .HasColumnName("email_address")
                    .HasMaxLength(50);

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.UpdateTime)
                    .HasColumnName("update_time")
                    .HasMaxLength(30)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Products>(entity =>
            {
                entity.HasKey(e => e.ProductId)
                    .HasName("PK__Products__2D10D16AE5C6F56A");

                entity.Property(e => e.ProductId).HasColumnName("productId");

                entity.Property(e => e.Calories).HasColumnName("calories");

                entity.Property(e => e.Carb).HasColumnName("carb");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("ntext");

                entity.Property(e => e.Fat).HasColumnName("fat");

                entity.Property(e => e.Image)
                    .HasColumnName("image")
                    .HasColumnType("ntext");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(40);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("money");

                entity.Property(e => e.Protein).HasColumnName("protein");
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.TokenId)
                    .HasName("PK__RefreshT__CB3C9E1728955239");

                entity.Property(e => e.TokenId).HasColumnName("token_id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.ExpiryDate)
                    .HasColumnName("expiry_date")
                    .HasColumnType("datetime");

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.RefreshToken)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_RefreshToken_Users");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__Users__CB9A1CFFC1841312");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(50);

                entity.Property(e => e.IsAdmin).HasColumnName("isAdmin");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(30);

                entity.Property(e => e.Password)
                    .HasColumnName("password")
                    .HasMaxLength(15);

                entity.Property(e => e.PhoneNumber)
                    .HasColumnName("phoneNumber")
                    .HasMaxLength(10);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
