﻿// <auto-generated />
using System;
using InventoryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InventoryManagementSystem.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.5");

            modelBuilder.Entity("InventoryManagementSystem.Models.Category", b =>
                {
                    b.Property<int>("IdCategory")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CategoryCode")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("TEXT");

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.HasKey("IdCategory");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Item", b =>
                {
                    b.Property<int>("IdItem")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Availability")
                        .HasColumnType("INTEGER");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("KodeItem")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SubCategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SupplierId")
                        .HasColumnType("INTEGER");

                    b.HasKey("IdItem");

                    b.HasIndex("CategoryId");

                    b.HasIndex("SubCategoryId");

                    b.HasIndex("SupplierId");

                    b.ToTable("Items");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.SubCategory", b =>
                {
                    b.Property<int>("IdSubCategory")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("CategoryIdCategory")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.Property<int>("IdCategory")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SubCategoryCode")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("TEXT");

                    b.Property<string>("SubCategoryName")
                        .IsRequired()
                        .HasMaxLength(15)
                        .HasColumnType("TEXT");

                    b.HasKey("IdSubCategory");

                    b.HasIndex("CategoryIdCategory");

                    b.ToTable("SubCategories");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Supplier", b =>
                {
                    b.Property<int>("SupplierId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.Property<string>("City")
                        .HasMaxLength(60)
                        .HasColumnType("TEXT");

                    b.Property<string>("CompanyName")
                        .IsRequired()
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.Property<string>("ContactName")
                        .HasMaxLength(40)
                        .HasColumnType("TEXT");

                    b.HasKey("SupplierId");

                    b.ToTable("Suppliers");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Item", b =>
                {
                    b.HasOne("InventoryManagementSystem.Models.Category", "Category")
                        .WithMany("Items")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InventoryManagementSystem.Models.SubCategory", "SubCategory")
                        .WithMany()
                        .HasForeignKey("SubCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("InventoryManagementSystem.Models.Supplier", "Supplier")
                        .WithMany("Items")
                        .HasForeignKey("SupplierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("SubCategory");

                    b.Navigation("Supplier");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.SubCategory", b =>
                {
                    b.HasOne("InventoryManagementSystem.Models.Category", "Category")
                        .WithMany("SubCategories")
                        .HasForeignKey("CategoryIdCategory");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Category", b =>
                {
                    b.Navigation("Items");

                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("InventoryManagementSystem.Models.Supplier", b =>
                {
                    b.Navigation("Items");
                });
#pragma warning restore 612, 618
        }
    }
}
