using System.Collections;
using System.Collections.Generic;

namespace OmegaProject.Entity
{
    public class OpendGroupMessage
    {
        public int Id { get; set; }
        public int MessageId { get; set; }
        public int UserId { get; set; }

        public virtual GroupMessage Message { get; set; }
    }
}
