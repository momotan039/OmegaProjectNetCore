using System.Collections.Generic;

namespace OmegaProject.DTO
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        
        public virtual Course Course { get; set; }
    }
}
