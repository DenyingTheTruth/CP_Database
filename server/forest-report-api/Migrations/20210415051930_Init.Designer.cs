﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using forest_report_api.Context;

namespace forest_report_api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20210415051930_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.4")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("forest_report_api.Entities.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("forest_report_api.Entities.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("Fio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("OrganizationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PasswordEncrypt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("OrganizationId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("forest_report_api.Entities.AttachmentFile", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<byte[]>("Value")
                        .HasColumnType("varbinary(max)");

                    b.HasKey("Id");

                    b.ToTable("Files");
                });

            modelBuilder.Entity("forest_report_api.Entities.BaseFormRep", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("CollectionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Json")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NoValid")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("TabId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Tabs");
                });

            modelBuilder.Entity("forest_report_api.Entities.LogReport", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("AdminStatusReport")
                        .HasColumnType("int");

                    b.Property<string>("ApplicationUserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReportId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("StatusReport")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.HasIndex("ReportId");

                    b.ToTable("LogReports");
                });

            modelBuilder.Entity("forest_report_api.Entities.Organization", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GovermentForReport")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsHolding")
                        .HasColumnType("bit");

                    b.Property<bool>("IsState")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizationalLegalForm")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Position2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Region")
                        .HasColumnType("int");

                    b.Property<string>("TypeActivityId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("TypeEconomicActivity")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UNP")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UnitForReport")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("TypeActivityId");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("forest_report_api.Entities.Period", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Periods");
                });

            modelBuilder.Entity("forest_report_api.Entities.PeriodReportType", b =>
                {
                    b.Property<string>("PeriodId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ReportTypeId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("PeriodId", "ReportTypeId");

                    b.HasIndex("ReportTypeId");

                    b.ToTable("PeriodReportTypes");
                });

            modelBuilder.Entity("forest_report_api.Entities.Report", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int?>("AdminStatusReport")
                        .HasColumnType("int");

                    b.Property<string>("AttachmentFileId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FormCollectionId")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsRead")
                        .HasColumnType("bit");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ReplyDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("ReportTypeId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("ReturnDate")
                        .HasColumnType("datetime2");

                    b.Property<int?>("StatusReport")
                        .HasColumnType("int");

                    b.Property<string>("UserCheckinIntervalId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("AttachmentFileId");

                    b.HasIndex("ReportTypeId");

                    b.HasIndex("UserCheckinIntervalId");

                    b.HasIndex("UserId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("forest_report_api.Entities.ReportType", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("ReportTypes");
                });

            modelBuilder.Entity("forest_report_api.Entities.TypeActivity", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<bool>("IsIndustrial")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Position")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("TypeActivities");
                });

            modelBuilder.Entity("forest_report_api.Entities.UserCheckinInterval", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("OrganizationId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("PeriodId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Year")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("PeriodId");

                    b.ToTable("UserCheckinIntervals");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("forest_report_api.Entities.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("forest_report_api.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("forest_report_api.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("forest_report_api.Entities.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("forest_report_api.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("forest_report_api.Entities.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("forest_report_api.Entities.ApplicationUser", b =>
                {
                    b.HasOne("forest_report_api.Entities.Organization", "Organization")
                        .WithMany("Users")
                        .HasForeignKey("OrganizationId");

                    b.Navigation("Organization");
                });

            modelBuilder.Entity("forest_report_api.Entities.LogReport", b =>
                {
                    b.HasOne("forest_report_api.Entities.ApplicationUser", "ApplicationUser")
                        .WithMany()
                        .HasForeignKey("ApplicationUserId");

                    b.HasOne("forest_report_api.Entities.Report", "Report")
                        .WithMany()
                        .HasForeignKey("ReportId");

                    b.Navigation("ApplicationUser");

                    b.Navigation("Report");
                });

            modelBuilder.Entity("forest_report_api.Entities.Organization", b =>
                {
                    b.HasOne("forest_report_api.Entities.TypeActivity", "TypeActivity")
                        .WithMany()
                        .HasForeignKey("TypeActivityId");

                    b.Navigation("TypeActivity");
                });

            modelBuilder.Entity("forest_report_api.Entities.PeriodReportType", b =>
                {
                    b.HasOne("forest_report_api.Entities.Period", "Period")
                        .WithMany("ReportTypes")
                        .HasForeignKey("PeriodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("forest_report_api.Entities.ReportType", "ReportType")
                        .WithMany("Periods")
                        .HasForeignKey("ReportTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Period");

                    b.Navigation("ReportType");
                });

            modelBuilder.Entity("forest_report_api.Entities.Report", b =>
                {
                    b.HasOne("forest_report_api.Entities.AttachmentFile", "AttachmentFile")
                        .WithMany()
                        .HasForeignKey("AttachmentFileId");

                    b.HasOne("forest_report_api.Entities.ReportType", "ReportType")
                        .WithMany()
                        .HasForeignKey("ReportTypeId");

                    b.HasOne("forest_report_api.Entities.UserCheckinInterval", "UserCheckinInterval")
                        .WithMany()
                        .HasForeignKey("UserCheckinIntervalId");

                    b.HasOne("forest_report_api.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("AttachmentFile");

                    b.Navigation("ReportType");

                    b.Navigation("User");

                    b.Navigation("UserCheckinInterval");
                });

            modelBuilder.Entity("forest_report_api.Entities.UserCheckinInterval", b =>
                {
                    b.HasOne("forest_report_api.Entities.Organization", "Organization")
                        .WithMany()
                        .HasForeignKey("OrganizationId");

                    b.HasOne("forest_report_api.Entities.Period", "Period")
                        .WithMany()
                        .HasForeignKey("PeriodId");

                    b.Navigation("Organization");

                    b.Navigation("Period");
                });

            modelBuilder.Entity("forest_report_api.Entities.Organization", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("forest_report_api.Entities.Period", b =>
                {
                    b.Navigation("ReportTypes");
                });

            modelBuilder.Entity("forest_report_api.Entities.ReportType", b =>
                {
                    b.Navigation("Periods");
                });
#pragma warning restore 612, 618
        }
    }
}
