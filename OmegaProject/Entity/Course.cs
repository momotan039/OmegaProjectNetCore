using System.Collections.Generic;

namespace OmegaProject.DTO
{
    public class Course
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Group> groups { get; set; }
    }
}
