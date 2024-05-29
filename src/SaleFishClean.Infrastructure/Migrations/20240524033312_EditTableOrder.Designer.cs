﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SaleFishClean.Infrastructure.Data;

#nullable disable

namespace SaleFishClean.Infrastructure.Migrations
{
    [DbContext(typeof(SaleFishProjectContext))]
    [Migration("20240524033312_EditTableOrder")]
    partial class EditTableOrder
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.4")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("OrderProduct", b =>
                {
                    b.Property<string>("OrdersOrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("ProductsProductId")
                        .HasColumnType("int");

                    b.HasKey("OrdersOrderId", "ProductsProductId");

                    b.HasIndex("ProductsProductId");

                    b.ToTable("OrderProduct");
                });

            modelBuilder.Entity("ProductSupplier", b =>
                {
                    b.Property<int>("ProductsProductsProductId")
                        .HasColumnType("int");

                    b.Property<int>("SuppliersSuppliersSupplierId")
                        .HasColumnType("int");

                    b.HasKey("ProductsProductsProductId", "SuppliersSuppliersSupplierId");

                    b.HasIndex("SuppliersSuppliersSupplierId");

                    b.ToTable("ProductSupplier");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Brand", b =>
                {
                    b.Property<int>("BrandId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("BrandID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("BrandId"));

                    b.Property<string>("BrandName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("CountryOfOrigin")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateOnly?>("FoundedDate")
                        .HasColumnType("date");

                    b.HasKey("BrandId")
                        .HasName("PK__Brand__DAD4F3BE05D8207B");

                    b.ToTable("Brand", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Comment", b =>
                {
                    b.Property<int>("CommentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CommentID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CommentId"));

                    b.Property<DateTime?>("CommentDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Content")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("ProductID");

                    b.Property<string>("UserId")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("varchar(5)")
                        .HasColumnName("UserID");

                    b.HasKey("CommentId")
                        .HasName("PK__Comment__C3B4DFAAE86E235E");

                    b.HasIndex(new[] { "ProductId" }, "IX_Comment_ProductID");

                    b.HasIndex(new[] { "UserId" }, "IX_Comment_UserID");

                    b.ToTable("Comment", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Discount", b =>
                {
                    b.Property<int>("DiscountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("DiscountID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("DiscountId"));

                    b.Property<string>("Code")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<decimal?>("DiscountValue")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime");

                    b.HasKey("DiscountId")
                        .HasName("PK__Discount__E43F6DF69D5F9636");

                    b.ToTable("Discount", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Inventory", b =>
                {
                    b.Property<int>("InventoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("InventoryID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("InventoryId"));

                    b.Property<DateTime>("EntryDate")
                        .HasColumnType("datetime");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("ProductID");

                    b.Property<int?>("Quantity")
                        .HasColumnType("int");

                    b.Property<int?>("SupplierId")
                        .HasColumnType("int")
                        .HasColumnName("SupplierID");

                    b.HasKey("InventoryId")
                        .HasName("PK__Inventor__F5FDE6D3DFEBBA93");

                    b.HasIndex(new[] { "ProductId" }, "IX_Inventory_ProductID");

                    b.HasIndex(new[] { "SupplierId" }, "IX_Inventory_SupplierID");

                    b.ToTable("Inventory", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.InventoryRecord", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RecordID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecordId"));

                    b.Property<int?>("InventoryId")
                        .HasColumnType("int")
                        .HasColumnName("InventoryID");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<DateTime>("RecordDate")
                        .HasColumnType("datetime");

                    b.Property<string>("RecordType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("RecordId")
                        .HasName("PK__Inventor__FBDF78C947CB5ACA");

                    b.HasIndex(new[] { "InventoryId" }, "IX_InventoryRecord_InventoryID");

                    b.ToTable("InventoryRecord", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Order", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("OrderID");

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("LastName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime>("OrderDate")
                        .HasColumnType("datetime");

                    b.Property<string>("PaymentMethod")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int?>("PromotionId")
                        .HasColumnType("int")
                        .HasColumnName("PromotionID");

                    b.Property<int?>("Status")
                        .HasColumnType("int")
                        .HasColumnName("Status");

                    b.Property<string>("UserId")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("varchar(5)")
                        .HasColumnName("UserID");

                    b.HasKey("OrderId")
                        .HasName("PK__Orders__C3905BAF39564073");

                    b.HasIndex(new[] { "PromotionId" }, "IX_Orders_PromotionID");

                    b.HasIndex(new[] { "UserId" }, "IX_Orders_UserID");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.OrderDetail", b =>
                {
                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("OrderID");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("ProductID");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10, 2)")
                        .HasColumnName("Price");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("Quantity");

                    b.HasKey("OrderId", "ProductId")
                        .HasName("PK__OrderDet__08D097C14B7319A7");

                    b.HasIndex(new[] { "ProductId" }, "IX_OrderDetail_ProductID");

                    b.ToTable("OrderDetail", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PostID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PostId"));

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("PostedDate")
                        .HasColumnType("datetime");

                    b.Property<string>("Title")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("UserId")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("varchar(5)")
                        .HasColumnName("UserID");

                    b.HasKey("PostId")
                        .HasName("PK__Post__AA126038FFA166A6");

                    b.HasIndex(new[] { "UserId" }, "IX_Post_UserID");

                    b.ToTable("Post", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ProductID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<int?>("BrandId")
                        .HasColumnType("int")
                        .HasColumnName("BrandID");

                    b.Property<string>("Color")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("DiscountId")
                        .HasColumnType("int")
                        .HasColumnName("DiscountID");

                    b.Property<bool?>("HasSpecialFeatures")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsBestseller")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsNew")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsOnSale")
                        .HasColumnType("bit");

                    b.Property<string>("Manufacturer")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10, 2)");

                    b.Property<string>("ProductImage")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int?>("ProductTypeId")
                        .HasColumnType("int")
                        .HasColumnName("ProductTypeID");

                    b.Property<string>("SpecialNote")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("SupplierId")
                        .HasColumnType("int");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Weight")
                        .HasColumnType("decimal(10, 2)");

                    b.HasKey("ProductId")
                        .HasName("PK__Product__B40CC6ED8A40D83B");

                    b.HasIndex(new[] { "BrandId" }, "IX_Product_BrandID");

                    b.HasIndex(new[] { "DiscountId" }, "IX_Product_DiscountID");

                    b.HasIndex(new[] { "ProductTypeId" }, "IX_Product_ProductTypeID");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.ProductType", b =>
                {
                    b.Property<int>("ProductTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("ProductTypeID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductTypeId"));

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ProductTypeName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("ProductTypeId")
                        .HasName("PK__ProductT__A1312F4E6B49C550");

                    b.ToTable("ProductType", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Promotion", b =>
                {
                    b.Property<int>("PromotionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("PromotionID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PromotionId"));

                    b.Property<decimal?>("DiscountRate")
                        .HasColumnType("decimal(5, 2)");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime");

                    b.Property<string>("PromotionName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("StartDate")
                        .HasColumnType("datetime");

                    b.HasKey("PromotionId")
                        .HasName("PK__Promotio__52C42F2F83AAB341");

                    b.ToTable("Promotion", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReplaceByToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Revoked")
                        .HasColumnType("datetime2");

                    b.Property<string>("TokenRefresh")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("varchar(5)");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "UserId" }, "IX_RefreshTokens_UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.ShipMoney", b =>
                {
                    b.Property<string>("ShippingId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<decimal?>("ShippingUnitPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("ShippingId");

                    b.ToTable("ShipMoneys");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Shipper", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(5)");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("datetime");

                    b.HasKey("UserId", "OrderId");

                    b.HasIndex("OrderId");

                    b.ToTable("Shippers");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.ShoppingCart", b =>
                {
                    b.Property<string>("CartId")
                        .HasColumnType("varchar(7)")
                        .HasColumnName("CartID");

                    b.Property<DateTime?>("CreateDate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime?>("LastUpdate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("UserId")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("varchar(5)")
                        .HasColumnName("UserID");

                    b.HasKey("CartId")
                        .HasName("PK__Shopping__51BCD797F024C1F5");

                    b.HasIndex(new[] { "UserId" }, "IX_ShoppingCart_UserID");

                    b.ToTable("ShoppingCart", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.ShoppingCartDetail", b =>
                {
                    b.Property<string>("CartId")
                        .HasColumnType("varchar(7)")
                        .HasColumnName("CartID");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("ProductID");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("CartId", "ProductId")
                        .HasName("PK__Shopping__9AFC1BF92693E3E3");

                    b.HasIndex(new[] { "ProductId" }, "IX_ShoppingCartDetail_ProductID");

                    b.ToTable("ShoppingCartDetail", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Supplier", b =>
                {
                    b.Property<int>("SupplierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("SupplierID");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SupplierId"));

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("ContactEmail")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ContactPerson")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<DateTime>("RegistrationDate")
                        .HasColumnType("datetime");

                    b.Property<string>("SupplierName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("SupplierId")
                        .HasName("PK__Supplier__4BE66694219EC1BC");

                    b.ToTable("Supplier", (string)null);
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("varchar(5)")
                        .HasColumnName("UserID");

                    b.Property<string>("Address")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<DateTime?>("BlockedTime")
                        .HasColumnType("datetime");

                    b.Property<string>("Cccd")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)")
                        .HasColumnName("CCCD");

                    b.Property<string>("CodeOtp")
                        .HasMaxLength(5)
                        .IsUnicode(false)
                        .HasColumnType("varchar(5)")
                        .HasColumnName("CodeOTP");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool?>("Gender")
                        .HasColumnType("bit");

                    b.Property<string>("ImageName")
                        .HasColumnType("varchar(Max)");

                    b.Property<bool?>("IsBlocked")
                        .HasColumnType("bit");

                    b.Property<bool?>("IsVerify")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("OtpCreate")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("OtpExpired")
                        .HasColumnType("datetime");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PhoneNumber")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<DateTime?>("ResetTokenExpires")
                        .HasColumnType("datetime");

                    b.Property<string>("RollName")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("varchar(20)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .IsUnicode(false)
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId")
                        .HasName("PK__Users__1788CCAC1A5611C6");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("OrderProduct", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.Order", null)
                        .WithMany()
                        .HasForeignKey("OrdersOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SaleFishClean.Domains.Entities.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ProductSupplier", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.Product", null)
                        .WithMany()
                        .HasForeignKey("ProductsProductsProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SaleFishClean.Domains.Entities.Supplier", null)
                        .WithMany()
                        .HasForeignKey("SuppliersSuppliersSupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Comment", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.Product", "Product")
                        .WithMany("Comments")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK__Comment__Product__5BAD9CC8");

                    b.HasOne("SaleFishClean.Domains.Entities.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Comment__UserID__5CA1C101");

                    b.Navigation("Product");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Inventory", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.Product", "Product")
                        .WithMany("Inventories")
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK__Inventory__Produ__4C6B5938");

                    b.HasOne("SaleFishClean.Domains.Entities.Supplier", "Supplier")
                        .WithMany("Inventories")
                        .HasForeignKey("SupplierId")
                        .HasConstraintName("FK__Inventory__Suppl__4B7734FF");

                    b.Navigation("Product");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.InventoryRecord", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.Inventory", "Inventory")
                        .WithMany("InventoryRecords")
                        .HasForeignKey("InventoryId")
                        .HasConstraintName("FK__Inventory__Inven__4F47C5E3");

                    b.Navigation("Inventory");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Order", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.Promotion", "Promotion")
                        .WithMany("Orders")
                        .HasForeignKey("PromotionId")
                        .HasConstraintName("FK__Orders__Promotio__55009F39");

                    b.HasOne("SaleFishClean.Domains.Entities.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Orders__UserID__540C7B00");

                    b.Navigation("Promotion");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.OrderDetail", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.Order", "Order")
                        .WithMany("OrderDetails")
                        .HasForeignKey("OrderId")
                        .IsRequired()
                        .HasConstraintName("FK__OrderDeta__Order__57DD0BE4");

                    b.HasOne("SaleFishClean.Domains.Entities.Product", "Product")
                        .WithMany("OrderDetails")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK__OrderDeta__Produ__58D1301D");

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Post", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.User", "User")
                        .WithMany("Posts")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__Post__UserID__625A9A57");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Product", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.Brand", "Brand")
                        .WithMany("Products")
                        .HasForeignKey("BrandId")
                        .HasConstraintName("FK__Product__BrandID__43D61337");

                    b.HasOne("SaleFishClean.Domains.Entities.Discount", "Discount")
                        .WithMany("Products")
                        .HasForeignKey("DiscountId")
                        .HasConstraintName("FK__Product__Discoun__45BE5BA9");

                    b.HasOne("SaleFishClean.Domains.Entities.ProductType", "ProductType")
                        .WithMany("Products")
                        .HasForeignKey("ProductTypeId")
                        .HasConstraintName("FK__Product__Product__44CA3770");

                    b.Navigation("Brand");

                    b.Navigation("Discount");

                    b.Navigation("ProductType");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.RefreshToken", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Shipper", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SaleFishClean.Domains.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.ShoppingCart", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.User", "User")
                        .WithMany("ShoppingCarts")
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK__ShoppingC__UserI__681373AD");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.ShoppingCartDetail", b =>
                {
                    b.HasOne("SaleFishClean.Domains.Entities.ShoppingCart", "Cart")
                        .WithMany("ShoppingCartDetails")
                        .HasForeignKey("CartId")
                        .IsRequired()
                        .HasConstraintName("FK__ShoppingC__CartI__6AEFE058");

                    b.HasOne("SaleFishClean.Domains.Entities.Product", "Product")
                        .WithMany("ShoppingCartDetails")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK__ShoppingC__Produ__6BE40491");

                    b.Navigation("Cart");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Brand", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Discount", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Inventory", b =>
                {
                    b.Navigation("InventoryRecords");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Order", b =>
                {
                    b.Navigation("OrderDetails");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Product", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Inventories");

                    b.Navigation("OrderDetails");

                    b.Navigation("ShoppingCartDetails");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.ProductType", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Promotion", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.ShoppingCart", b =>
                {
                    b.Navigation("ShoppingCartDetails");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.Supplier", b =>
                {
                    b.Navigation("Inventories");
                });

            modelBuilder.Entity("SaleFishClean.Domains.Entities.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("Orders");

                    b.Navigation("Posts");

                    b.Navigation("RefreshTokens");

                    b.Navigation("ShoppingCarts");
                });
#pragma warning restore 612, 618
        }
    }
}
