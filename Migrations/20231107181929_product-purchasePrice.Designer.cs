﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StockManagement.Models;

#nullable disable

namespace StockManagement.Migrations
{
    [DbContext(typeof(StockDbContext))]
    [Migration("20231107181929_product-purchasePrice")]
    partial class productpurchasePrice
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.24")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("StockManagement.Models.Product", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"), 1L, 1);

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("PurchaseItemId")
                        .HasColumnType("int");

                    b.Property<decimal>("PurchasePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("SalesItemId")
                        .HasColumnType("int");

                    b.HasKey("ProductId");

                    b.HasIndex("PurchaseItemId");

                    b.HasIndex("SalesItemId");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("StockManagement.Models.Purchase", b =>
                {
                    b.Property<int>("PurchaseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseId"), 1L, 1);

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("SupplierName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PurchaseId");

                    b.ToTable("Purchases");
                });

            modelBuilder.Entity("StockManagement.Models.PurchaseItem", b =>
                {
                    b.Property<int>("PurchaseItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("PurchaseItemId"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("PurchaseId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.HasKey("PurchaseItemId");

                    b.HasIndex("ProductId");

                    b.HasIndex("PurchaseId")
                        .IsUnique();

                    b.ToTable("PurchaseItem");
                });

            modelBuilder.Entity("StockManagement.Models.Sale", b =>
                {
                    b.Property<int>("SalesId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SalesId"), 1L, 1);

                    b.Property<string>("CustomerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.HasKey("SalesId");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("StockManagement.Models.SalesItem", b =>
                {
                    b.Property<int>("SalesItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("SalesItemId"), 1L, 1);

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("SalesId")
                        .HasColumnType("int");

                    b.HasKey("SalesItemId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SalesId")
                        .IsUnique();

                    b.ToTable("SalesItem");
                });

            modelBuilder.Entity("StockManagement.Models.Stock", b =>
                {
                    b.Property<int>("StockId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("StockId"), 1L, 1);

                    b.Property<int>("ProductId")
                        .HasColumnType("int");

                    b.Property<int>("TotalQuantity")
                        .HasColumnType("int");

                    b.HasKey("StockId");

                    b.HasIndex("ProductId");

                    b.ToTable("Stocks");
                });

            modelBuilder.Entity("StockManagement.Models.Product", b =>
                {
                    b.HasOne("StockManagement.Models.PurchaseItem", null)
                        .WithMany("Products")
                        .HasForeignKey("PurchaseItemId");

                    b.HasOne("StockManagement.Models.SalesItem", null)
                        .WithMany("Products")
                        .HasForeignKey("SalesItemId");
                });

            modelBuilder.Entity("StockManagement.Models.PurchaseItem", b =>
                {
                    b.HasOne("StockManagement.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StockManagement.Models.Purchase", "Purchase")
                        .WithOne("PurchaseItem")
                        .HasForeignKey("StockManagement.Models.PurchaseItem", "PurchaseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Purchase");
                });

            modelBuilder.Entity("StockManagement.Models.SalesItem", b =>
                {
                    b.HasOne("StockManagement.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("StockManagement.Models.Sale", "Sale")
                        .WithOne("SalesItem")
                        .HasForeignKey("StockManagement.Models.SalesItem", "SalesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");

                    b.Navigation("Sale");
                });

            modelBuilder.Entity("StockManagement.Models.Stock", b =>
                {
                    b.HasOne("StockManagement.Models.Product", "Product")
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("StockManagement.Models.Purchase", b =>
                {
                    b.Navigation("PurchaseItem");
                });

            modelBuilder.Entity("StockManagement.Models.PurchaseItem", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("StockManagement.Models.Sale", b =>
                {
                    b.Navigation("SalesItem");
                });

            modelBuilder.Entity("StockManagement.Models.SalesItem", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
