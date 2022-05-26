
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;

namespace OmegaProject.services
{
    public class MyDbContext:DbContext
    {
        //These Objects will connecting with database table by names
        //      ------     so if change object name ,it will not recognize them     -------
        public DbSet<UserDTO> Users { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Group> Gropus { get; set; }
        public MyDbContext(DbContextOptions dbContextOption):base(dbContextOption)
        {

        }

    }
}
