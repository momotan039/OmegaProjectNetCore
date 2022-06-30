using Microsoft.AspNetCore.Http;
using System;

namespace OmegaProject.DTO
{
    public class HomeWorkDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Contents { get; set; }
        public IFormFile FilesPath { get; set; }
        public int GroupId { get; set; }
        public DateTime dateTime { get; set; }
        public virtual Group Group { get; set; }
    }
}
