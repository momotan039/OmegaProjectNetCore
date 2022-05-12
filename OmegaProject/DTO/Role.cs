using System.ComponentModel.DataAnnotations;

namespace OmegaProject.DTO
{
    public class Role
    {
        [Key]
        public int NumberRole { get; set; }
        public string Description { get; set; }
    }
}
