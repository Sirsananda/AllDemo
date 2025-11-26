using AllDemo.Models;
using Microsoft.EntityFrameworkCore;

namespace AllDemo.Data.context
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
        public DbSet<SemesterModel> Semesters { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<AdmissionTypeModel>().HasIndex(a => a.AdmissionTypeName).IsUnique();//define a unique key here
            //modelBuilder.Entity<SemesterModel>().HasNoKey();// if you do not have a primary key on a table
        }
    }
}
