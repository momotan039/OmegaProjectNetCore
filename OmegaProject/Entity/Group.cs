using System;
using System.Collections.Generic;

namespace OmegaProject.DTO
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public DateTime? OpeningDate { get; set; } 
        public DateTime? ClosingDate { get; set; } 
        public virtual Course Course { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
        //public virtual ICollection<User> Users { get; set; }
        //public virtual List<UserGroup> UserRelation { get; set; }
    }
}
