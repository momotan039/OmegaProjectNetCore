using OmegaProject.DTO;
using System;

namespace OmegaProject.Entity
{
    public class Attendance
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public int GroupId { get; set; }
        public string Note { get; set; }
        public bool Status { get; set; }



        public virtual User Student { get; set; }
        public virtual Group Group { get; set; }
    }
}
