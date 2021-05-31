using forest_report_api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace forest_report_api.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=DESKTOP-QK0BQRG;database=forest-report;uid=sa;password=Qwerty1");
        }

        public ApplicationDbContext()
        {
        }

        public DbSet<TypeActivity> TypeActivities { get; set; }
        public DbSet<ReportType> ReportTypes { get; set; }
        public DbSet<Period> Periods { get; set; }
        public DbSet<UserCheckinInterval> UserCheckinIntervals { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<LogReport> LogReports { get; set; }
        public DbSet<BaseFormRep> Tabs { get; set; }
        public DbSet<PeriodReportType> PeriodReportTypes { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<AttachmentFile> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PeriodReportType>()
                .HasKey(t => new {t.PeriodId, t.ReportTypeId});

            modelBuilder.Entity<PeriodReportType>()
                .HasOne(pr => pr.Period)
                .WithMany(r => r.ReportTypes)
                .HasForeignKey(pr => pr.PeriodId);

            modelBuilder.Entity<PeriodReportType>()
                .HasOne(x => x.ReportType)
                .WithMany(c => c.Periods)
                .HasForeignKey(sc => sc.ReportTypeId);
        }
    }
}