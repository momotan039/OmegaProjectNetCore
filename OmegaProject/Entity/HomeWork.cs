using OmegaProject.DTO;
using System;

namespace OmegaProject.Entity
{
    public class HomeWork
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public string FilesPath { get; set; }
        public int GroupId { get; set; }
        public int TeacherId { get; set; }
        public bool RequiredSubmit { get; set; }

        public DateTime SendingDate { get; set; }
        public virtual Group Group { get; set; }
        public virtual User Teacher { get; set; }
    }
}
