
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.Entity;

namespace OmegaProject.services
{
    public class MyDbContext:DbContext
    {
        //These Objects will connecting with database table by names
        //      ------     so if change object name ,it will not recognize them     -------
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UsersGroups { get; set; }
        public DbSet<HomeWork> HomeWorks { get; set; }
        public DbSet<HomeWorkStudent> HomeWorkStudents { get; set; }
        private static DbContextOptions options;
        public  MyDbContext(DbContextOptions dbContextOption):base(dbContextOption)
        {
            MyDbContext.options= dbContextOption;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //dclare relartionship one to many=>User have many messages
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Reciver)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.ReciverId);

         

            //modelBuilder.Entity<UserGroup>().HasKey(ug=>new
            //{
            //    ug.UserId,
            //    ug.GroupId
            //});
            //modelBuilder.Entity<UserGroup>()
            //    .HasOne(ug=>ug.User)
            //    .WithMany(ug => ug.Groups)
            //    .HasForeignKey(ug=>ug.UserId);

            //modelBuilder.Entity<UserGroup>()
            //  .HasOne(ug => ug.Group)
            //  .WithMany(ug => ug.Users)
            //  .HasForeignKey(ug => ug.GroupId);
        }
      
        public static MyDbContext getInctence()
        {
            return new MyDbContext(options);
        }
    }
}
