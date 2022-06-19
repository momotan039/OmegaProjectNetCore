
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;

namespace OmegaProject.services
{
    public class MyDbContext:DbContext
    {
        //These Objects will connecting with database table by names
        //      ------     so if change object name ,it will not recognize them     -------
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UsersGroups { get; set; }
        public MyDbContext(DbContextOptions dbContextOption):base(dbContextOption)
        {

        }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //dclare relartionship one to many without create extra table
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m=>m.SenderId);

            //modelBuilder.Entity<Group>().
            //    HasMany(g => g.Users)
            //    .WithMany(u => u.Groups);

         
        }

        }
}
