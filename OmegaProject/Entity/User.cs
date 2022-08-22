using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmegaProject.DTO
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int RoleId { get; set; }
        public string IdCard { get; set; }
        public bool ConfirmPassword { get; set; }

       //public virtual int GroupId { get; set; }

        public virtual Role Role { get; set; }
        public virtual List<Message> Messages { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }

    }

}
