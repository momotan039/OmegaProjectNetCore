using System;

namespace OmegaProject.DTO
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Contents { get; set; }
        public string Title { get; set; }
        //public int GroupId { get; set; }
        public int SenderId { get; set; }
        public int ReciverId { get; set; }
        public bool IsOpened { get; set; }
        public DateTime SendingDate { get ; set; }  
        public  User Sender { get; set; }
    }
}
