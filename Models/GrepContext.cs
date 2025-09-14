using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace grepapi.Models;

public partial class GrepContext : DbContext
{
    public GrepContext()
    {
    }

    public GrepContext(DbContextOptions<GrepContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Catalog> Catalogs { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<CompanyActivation> CompanyActivations { get; set; }

    public virtual DbSet<CompanyPromoRequest> CompanyPromoRequests { get; set; }

    public virtual DbSet<DetailsLog> DetailsLogs { get; set; }

    public virtual DbSet<Market> Markets { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<SearchLog> SearchLogs { get; set; }

    public virtual DbSet<Shop> Shops { get; set; }

    public virtual DbSet<TradeSegment> TradeSegments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=51.250.102.249;Database=grep;Username=postgres;Password=yayqaCIEw20?9B?CTO5PEj");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Catalog>(entity =>
        {
            entity.HasKey(e => e.CatalogId).HasName("catalog_pk");

            entity.ToTable("catalog");

            entity.Property(e => e.CatalogId).HasColumnName("catalog_id");
            entity.Property(e => e.CatalogName)
                .HasMaxLength(100)
                .HasColumnName("catalog_name");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("company_pk");

            entity.ToTable("company");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.BusinessAddress)
                .HasMaxLength(1000)
                .HasColumnName("business_address");
            entity.Property(e => e.BusinessEmail)
                .HasMaxLength(50)
                .HasColumnName("business_email");
            entity.Property(e => e.BusinessMobile)
                .HasMaxLength(50)
                .HasColumnName("business_mobile");
            entity.Property(e => e.CompanyName)
                .HasMaxLength(500)
                .HasColumnName("company_name");
            entity.Property(e => e.CompanyPassword)
                .HasMaxLength(50)
                .HasColumnName("company_password");
            entity.Property(e => e.CompanyType)
                .HasDefaultValue(0)
                .HasColumnName("company_type");
            entity.Property(e => e.FriendInn)
                .HasMaxLength(50)
                .HasColumnName("friend_inn");
            entity.Property(e => e.FriendPhone)
                .HasMaxLength(50)
                .HasColumnName("friend_phone");
            entity.Property(e => e.Inn)
                .HasMaxLength(50)
                .HasColumnName("inn");
            entity.Property(e => e.RegistrationTs)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("registration_ts");
        });

        modelBuilder.Entity<CompanyActivation>(entity =>
        {
            entity.HasKey(e => e.ActivationId).HasName("company_activation_pk");

            entity.ToTable("company_activation");

            entity.Property(e => e.ActivationId).HasColumnName("activation_id");
            entity.Property(e => e.ActivationEnd)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("activation_end");
            entity.Property(e => e.ActivationStart)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("activation_start");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.FriendsPromo)
                .HasDefaultValue(0)
                .HasColumnName("friends_promo");
            entity.Property(e => e.Payment)
                .HasDefaultValue(0)
                .HasColumnName("payment");
            entity.Property(e => e.PaymentAmount).HasColumnName("payment_amount");
            entity.Property(e => e.PaymentDescription)
                .HasMaxLength(500)
                .HasColumnName("payment_description");
            entity.Property(e => e.PaymentTimestamp)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("payment_timestamp");
            entity.Property(e => e.Start)
                .HasDefaultValue(0)
                .HasColumnName("start");
        });

        modelBuilder.Entity<CompanyPromoRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("company_promo_request_pk");

            entity.ToTable("company_promo_request");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(1)
                .HasColumnName("active");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Inn)
                .HasMaxLength(50)
                .HasColumnName("inn");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Ts)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ts");
        });

        modelBuilder.Entity<DetailsLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("details_log_pk");

            entity.ToTable("details_log");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.MarketId).HasColumnName("market_id");
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ShopId).HasColumnName("shop_id");
            entity.Property(e => e.Ts)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ts");
        });

        modelBuilder.Entity<Market>(entity =>
        {
            entity.HasKey(e => e.MarketId).HasName("market_pk");

            entity.ToTable("market");

            entity.Property(e => e.MarketId)
                .ValueGeneratedNever()
                .HasColumnName("market_id");
            entity.Property(e => e.GeoLink)
                .HasMaxLength(1000)
                .HasColumnName("geo_link");
            entity.Property(e => e.LocationGeo).HasColumnName("location_geo");
            entity.Property(e => e.MarketName)
                .HasMaxLength(500)
                .HasColumnName("market_name");
            entity.Property(e => e.MarketWeb)
                .HasMaxLength(1000)
                .HasColumnName("market_web");
            entity.Property(e => e.ProfileType)
                .HasDefaultValue(2L)
                .HasColumnName("profile_type");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);

            entity.ToTable("product");

            entity.HasIndex(e => e.ProductId, "product_unique").IsUnique();

            entity.Property(e => e.Available).HasColumnName("available").HasDefaultValue(-1);
            entity.Property(e => e.Brand)
                .HasMaxLength(200)
                .HasColumnName("brand");
            entity.Property(e => e.CatalogId).HasColumnName("catalog_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.HighResHeight)
                .HasDefaultValue(600)
                .HasColumnName("high_res_height");
            entity.Property(e => e.HighResPicture)
                .HasMaxLength(50)
                .HasColumnName("high_res_picture");
            entity.Property(e => e.HighResPictureExt)
                .HasMaxLength(5)
                .HasColumnName("high_res_picture_ext");
            entity.Property(e => e.HighResWidth)
                .HasDefaultValue(800)
                .HasColumnName("high_res_width");
            entity.Property(e => e.ManufacturerUrl)
                .HasMaxLength(1000)
                .HasColumnName("manufacturer_url");
            entity.Property(e => e.MarketId).HasColumnName("market_id");
            entity.Property(e => e.Picture).HasColumnName("picture");
            entity.Property(e => e.PictureExt)
                .HasMaxLength(5)
                .HasColumnName("picture_ext");
            entity.Property(e => e.PictureHeight)
                .HasDefaultValue(300)
                .HasColumnName("picture_height");
            entity.Property(e => e.PictureWidth)
                .HasDefaultValue(400)
                .HasColumnName("picture_width");
            entity.Property(e => e.Price).HasColumnName("price").HasDefaultValue(0);
            entity.Property(e => e.ProductId).HasColumnName("product_id");
            entity.Property(e => e.ProductName).HasColumnName("product_name");
            entity.Property(e => e.SerialNo)
                .HasMaxLength(50)
                .HasColumnName("serial_no");
            entity.Property(e => e.ShopId).HasColumnName("shop_id");
            entity.Ignore(e => e.ShopName);
            entity.Property(e => e.HighResPicture)
                .HasMaxLength(50)
                .HasColumnName("high_res_picture");
            entity.Property(e => e.HighResPictureExt)
                .HasMaxLength(5)
                .HasColumnName("high_res_picture_ext");
            entity.Property(e => e.Promo)
                 .HasDefaultValue(0)
                 .HasColumnName("promo");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("product_category_pk");

            entity.ToTable("product_category");

            entity.Property(e => e.CategoryId)
                .ValueGeneratedNever()
                .HasColumnName("category_id");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(500)
                .HasColumnName("category_name");
        });

        modelBuilder.Entity<SearchLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("search_log_pk");

            entity.ToTable("search_log");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Keywords)
                .HasMaxLength(500)
                .HasColumnName("keywords");
            entity.Property(e => e.MarketId).HasColumnName("market_id");
            entity.Property(e => e.PriceMax).HasColumnName("price_max");
            entity.Property(e => e.PriceMin).HasColumnName("price_min");
            entity.Property(e => e.ProductFound).HasColumnName("product_found");
            entity.Property(e => e.Ts)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("ts");
        });

        modelBuilder.Entity<Shop>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("shop_pk");

            entity.ToTable("shop");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Active)
                .HasDefaultValue(1)
                .HasColumnName("active");
            entity.Property(e => e.CatalogId).HasColumnName("catalog_id");
            entity.Property(e => e.CompanyId)
                .HasDefaultValue(0L)
                .HasColumnName("company_id");
            entity.Property(e => e.MarketId)
                .HasColumnName("market_id");
            entity.Property(e => e.CustomerMobile)
                .HasMaxLength(50)
                .HasColumnName("customer_mobile");
            entity.Property(e => e.Email)
                .HasMaxLength(500)
                .HasColumnName("email");
            entity.Property(e => e.Inn)
                .HasMaxLength(50)
                .HasColumnName("inn");
            entity.Property(e => e.LocationGeo).HasColumnName("location_geo");
            entity.Property(e => e.LocationPic).HasColumnName("location_pic");
            entity.Property(e => e.LocationPicExt)
                .HasMaxLength(10)
                .HasColumnName("location_pic_ext");
            entity.Property(e => e.MarketId).HasColumnName("market_id");
            entity.Property(e => e.Mobile)
                .HasMaxLength(50)
                .HasColumnName("mobile");
            entity.Property(e => e.PathComments)
                .HasMaxLength(1000)
                .HasColumnName("path_comments");
            entity.Property(e => e.PathLink)
                .HasMaxLength(1000)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("path_link");
            entity.Property(e => e.ShopName).HasColumnName("shop_name");
            entity.Property(e => e.ShopType)
                .HasDefaultValue(0L)
                .HasColumnName("shop_type");
            entity.Property(e => e.ShopWeb)
                .HasMaxLength(500)
                .HasColumnName("shop_web");
            entity.Property(e => e.Telegram)
                .HasMaxLength(500)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("telegram");
            entity.Property(e => e.TradeSegmentId)
                .HasDefaultValue(1L)
                .HasColumnName("trade_segment_id");

            entity.Property(e => e.RegistrationDate)
              .HasColumnType("timestamp without time zone")
              .HasColumnName("registration_date");
        });

        modelBuilder.Entity<TradeSegment>(entity =>
        {
            entity.HasKey(e => e.SegmentId).HasName("newtable_pk");

            entity.ToTable("trade_segment");

            entity.Property(e => e.SegmentId)
                .ValueGeneratedNever()
                .HasColumnName("segment_id");
            entity.Property(e => e.SegmentName)
                .HasMaxLength(100)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("segment_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
