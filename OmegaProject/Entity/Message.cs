using System;

namespace OmegaProject.DTO
{
    public class Message
    {
        public int Id { get; set; }
        public string Contents { get; set; }
        //public string Title { get; set; }
        //public int GroupId { get; set; }
        public int SenderId { get; set; }
        public int ReciverId { get; set; }
        public bool IsOpened { get; set; }
        public DateTime? SendingDate { get; set; }

        public virtual User Sender { get; set; }
        public virtual User Reciver { get; set; }

    }
}
