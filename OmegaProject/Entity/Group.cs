using System;
using System.Collections.Generic;

namespace OmegaProject.DTO
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public DateTime? OpeningDate { get { return OpeningDate; } set { OpeningDate = DateTime.Now; } } 
        public DateTime? ClosingDate { get; set; } 
        public virtual Course Course { get; set; }
         public virtual ICollection<User> Users { get; set; }
    }
}
