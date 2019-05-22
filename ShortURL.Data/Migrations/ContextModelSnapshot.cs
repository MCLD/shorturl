﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShortURL.Data;

namespace ShortURL.Data.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("ShortURL.Model.Domain", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("GroupId");

                    b.HasKey("Name");

                    b.HasIndex("GroupId");

                    b.ToTable("Domains");
                });

            modelBuilder.Entity("ShortURL.Model.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("DefaultLink");

                    b.Property<string>("Description")
                        .HasMaxLength(255);

                    b.Property<bool>("IsDefault");

                    b.Property<DateTime?>("LatestVisit");

                    b.Property<int>("Visits");

                    b.HasKey("GroupId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("ShortURL.Model.GroupVisit", b =>
                {
                    b.Property<int>("GroupVisitId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("GroupId");

                    b.Property<DateTime>("VisitedAt");

                    b.HasKey("GroupVisitId");

                    b.ToTable("GroupVisits");
                });

            modelBuilder.Entity("ShortURL.Model.Record", b =>
                {
                    b.Property<int>("RecordId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int?>("GroupId");

                    b.Property<bool>("IsActive");

                    b.Property<DateTime?>("LatestVisit");

                    b.Property<string>("Link");

                    b.Property<string>("Stub")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("Visits");

                    b.HasKey("RecordId");

                    b.HasIndex("GroupId");

                    b.ToTable("Records");
                });

            modelBuilder.Entity("ShortURL.Model.RecordVisit", b =>
                {
                    b.Property<int>("RecordVisitId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RecordId");

                    b.Property<DateTime>("VisitedAt");

                    b.HasKey("RecordVisitId");

                    b.HasIndex("RecordId");

                    b.ToTable("RecordVisits");
                });

            modelBuilder.Entity("ShortURL.Model.Domain", b =>
                {
                    b.HasOne("ShortURL.Model.Group", "Group")
                        .WithMany("Domains")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("ShortURL.Model.Record", b =>
                {
                    b.HasOne("ShortURL.Model.Group")
                        .WithMany("Records")
                        .HasForeignKey("GroupId");
                });

            modelBuilder.Entity("ShortURL.Model.RecordVisit", b =>
                {
                    b.HasOne("ShortURL.Model.Record")
                        .WithMany("Accesses")
                        .HasForeignKey("RecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
