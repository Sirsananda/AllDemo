using AllDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace AllDemo.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<UserModel> UserModels { get; set; }
        public DbSet<RoleModel> RoleModels { get; set; }
        public DbSet<AdmissionTypeModel> AdmissionTypes { get; set; }
        public DbSet<AcademicYearModel> AcademicYears { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AdmissionTypeModel>().HasIndex(a => a.AdmissionTypeName).IsUnique();
        }
    }
}
