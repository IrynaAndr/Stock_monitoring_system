using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Stocks_monitoring_system_backend.Models
{
    public partial class MonitoringStocksContext : DbContext
    {
        public MonitoringStocksContext()
        {
        }

        public MonitoringStocksContext(DbContextOptions<MonitoringStocksContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Balance> Balances { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<StocksChange> StocksChanges { get; set; }
        public virtual DbSet<StocksTag> StocksTags { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserSold> UserSolds { get; set; }
        public virtual DbSet<UserStock> UserStocks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MonitoringStocks;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Balance>(entity =>
            {
                entity.ToTable("Balance");

                entity.Property(e => e.Date)
                    .HasColumnType("date")
                    .HasColumnName("date")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.IdUser).HasColumnName("Id_user");

                entity.Property(e => e.Msg)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("msg");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Balances)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Id_user4");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("notifications");

                entity.Property(e => e.IdUser).HasColumnName("Id_user");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnType("text")
                    .HasColumnName("text");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("type")
                    .HasDefaultValueSql("('common')");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Id_user5");
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.Property(e => e.CompanyName)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("Company_name");

                entity.Property(e => e.Info)
                    .HasColumnType("text")
                    .HasColumnName("info");

                entity.Property(e => e.MarketScope)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("market_scope");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("symbol");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("type")
                    .HasDefaultValueSql("('Common stocks')");
                entity.Property(e => e.NetIncome).HasColumnName("net_income");
                entity.Property(e => e.WeightedAverage).HasColumnName("weighted_average");
            });

            modelBuilder.Entity<StocksChange>(entity =>
            {
                entity.ToTable("Stocks_change");

                entity.Property(e => e.IdStocks).HasColumnName("Id_stocks");

                entity.Property(e => e.MarketValue).HasColumnName("market_value");

                entity.Property(e => e.StandardDeviation)
                    .HasColumnName("standard_deviation")
                    .HasDefaultValueSql("((0.1))");

                entity.Property(e => e.Weight).HasColumnName("weight");

                entity.HasOne(d => d.IdStocksNavigation)
                    .WithMany(p => p.StocksChanges)
                    .HasForeignKey(d => d.IdStocks)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Id_stocks");
            });

            modelBuilder.Entity<StocksTag>(entity =>
            {
                entity.ToTable("Stocks_tags");

                entity.Property(e => e.IdStocks).HasColumnName("Id_stocks");

                entity.Property(e => e.Tag)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("tag");

                entity.HasOne(d => d.IdStocksNavigation)
                    .WithMany(p => p.StocksTags)
                    .HasForeignKey(d => d.IdStocks)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Id_stocks2");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Login, "UQ__Users__5E55825BACA45FD7")
                    .IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Login)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

            
            });

            modelBuilder.Entity<UserSold>(entity =>
            {
                entity.ToTable("User_solds");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.Fee).HasColumnName("fee");

                entity.Property(e => e.IdStocks).HasColumnName("Id_stocks");

                entity.Property(e => e.IdUser).HasColumnName("Id_user");

                entity.Property(e => e.SellingDate)
                    .HasColumnType("date")
                    .HasColumnName("selling_date")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdStocksNavigation)
                    .WithMany(p => p.UserSolds)
                    .HasForeignKey(d => d.IdStocks)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Id_stocks4");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.UserSolds)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Id_user3");
            });

            modelBuilder.Entity<UserStock>(entity =>
            {
                entity.ToTable("User_stocks");

                entity.Property(e => e.Amount).HasColumnName("amount");

                entity.Property(e => e.Fee).HasColumnName("fee");

                entity.Property(e => e.IdStocks).HasColumnName("Id_stocks");

                entity.Property(e => e.IdUser).HasColumnName("Id_user");

                entity.Property(e => e.PurchaseDate)
                    .HasColumnType("date")
                    .HasColumnName("purchase_date")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.IdStocksNavigation)
                    .WithMany(p => p.UserStocks)
                    .HasForeignKey(d => d.IdStocks)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Id_stocks3");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.UserStocks)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Id_user2");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
