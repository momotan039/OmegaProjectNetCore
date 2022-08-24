using OmegaProject.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmegaProject.DTO
{
    public class Grade
    {
        public int Id { get; set; }
        [Column("Grade")]
        public double SumGrade { get; set; }
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public int TestId { get; set; }
        public string Note { get; set; }

        public virtual User Student { get; set; }
        public virtual Test Test { get; set; }
        public virtual Group Group { get; set; }

    }
}
