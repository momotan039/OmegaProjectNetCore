using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmegaProject.DTO
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public int Role { get; set; }
        public string IdCard { get; set; }

        //public int? GroupId { get; set; }

        public virtual List<Message> Messages { get; set; }
        //public virtual List<Group> Groups { get; set; }

        //public virtual List<UserGroup> GroupRelation { get; set; }
    }

}
