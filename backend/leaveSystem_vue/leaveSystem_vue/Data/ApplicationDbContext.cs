using Microsoft.EntityFrameworkCore;
using leaveSystem_vue.Models;

namespace leaveSystem_vue.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LoginModel> LoginModdel { get; set; }
        public DbSet<LeaveApplication> LeaveApplications { get; set; }
        public DbSet<OvertimeModel> Overtimes { get; set; }
        //public DbSet<SearchLeaveStatus> SearchLeaveStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LeaveApplication>().ToTable("Leave");
            modelBuilder.Entity<OvertimeModel>().ToTable("overtime_records");
            // 其他實體的配置...
        }
    }
}



//using Microsoft.EntityFrameworkCore;
//using leaveSystem_vue.Models;

//namespace leaveSystem_vue.Data
//{
//    public class ApplicationDbContext : DbContext
//    {
//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//            : base(options)
//        {
//        }

//        public DbSet<Employee> Employees { get; set; }
//        public DbSet<LoginModel> LoginModdel { get; set; }
//        public DbSet<LeaveApplication> LeaveApplications { get; set; }
//        public DbSet<OvertimeModel> Overtimes { get; set; }
//        //public DbSet<SearchLeaveStatus> SearchLeaveStatus { get; set; }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<LeaveApplication>().ToTable("Leave");
//            modelBuilder.Entity<OvertimeModel>().ToTable("overtime_records");
//            // 其他实体的配置...
//        }

//    }
//}
