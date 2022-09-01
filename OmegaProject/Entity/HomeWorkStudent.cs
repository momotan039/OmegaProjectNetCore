using OmegaProject.DTO;
using System.ComponentModel.DataAnnotations;

namespace OmegaProject.Entity
{
    public class HomeWorkStudent
    {
        public int Id { get; set; }
        public int HomeWorkId { get; set; }
        public int StudentId { get; set; }
        public string FilesPath { get; set; }
        
        public virtual HomeWork HomeWork { get; set; }  
        public virtual User Student { get; set; }  
    }
}
