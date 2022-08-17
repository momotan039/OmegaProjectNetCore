using OmegaProject.DTO;
using System;

namespace OmegaProject.Entity
{
    public class GroupMessage
    {
        public int Id { get; set; }
        public string Contents { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }
        public DateTime SendingDate { get; set; }

        public virtual Group Group { get; set; }
        public virtual User Sender { get; set; }
    }
}
