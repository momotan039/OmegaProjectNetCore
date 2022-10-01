using OmegaProject.DTO;
using System;
using System.Collections.Generic;

namespace OmegaProject.Entity
{
    public class GroupMessage
    {
        public int Id { get; set; }
        public string Contents { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }
        public DateTime SendingDate { get; set; }
        public bool IsOpened { get; set; }
        public virtual Group Group { get; set; }
        public virtual User Sender { get; set; }

        public virtual ICollection<OpendGroupMessage> OpendGroupMessages { get; set; }
    }
}
