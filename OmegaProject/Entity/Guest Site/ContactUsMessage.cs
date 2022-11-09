using System;

namespace OmegaProject.Entity
{
    public class ContactUsMessage
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime SendingDate { get; set; }

    }
}
