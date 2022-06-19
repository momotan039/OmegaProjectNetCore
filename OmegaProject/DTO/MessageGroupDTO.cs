using System;

namespace OmegaProject.DTO
{
    public class MessageGroupDTO
    {
        public int Id { get; set; }
        public string Contents { get; set; }
        public string Title { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }
        public DateTime SendingDate { get; set; }

    }
}
