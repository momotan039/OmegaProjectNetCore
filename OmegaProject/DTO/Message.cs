namespace OmegaProject.DTO
{
    public class Message
    {
        public int Id { get; set; }
        public string Contents { get; set; }
        public int GroupId { get; set; }
        public int SenderId { get; set; }
        public int ReciverId { get; set; }

    }
}
