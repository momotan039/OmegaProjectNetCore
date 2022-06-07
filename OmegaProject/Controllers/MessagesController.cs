

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        public MyDbContext db;
        public MessagesController(MyDbContext _db)
        {
            db = _db;
        }
        [HttpPost]
       [Route("SendMessage")]
       public IActionResult SendMessage([FromBody] Message msg)
        {
            db.Add(msg);
            db.SaveChanges();
            return  Ok("Message Sent Succsessfully");
        }
        [HttpPut]
        [Route("ChangeStatusMessage")]
        public IActionResult ChangeStatusMessage([FromBody] Message msg)
        {
            var temp=db.Messages.Find(msg.Id);
            if (temp == null)
                return BadRequest("Not found Message");
            temp.IsOpened = true;
            db.SaveChanges();
            return Ok("Message Changed Status Succsessfully");
        }

        [HttpGet]
        [Route("GetMessagesBySender/{id}")]
        public IActionResult GetMessage(int id)
        {
            var msgs = db.Messages.Where(msg => msg.SenderId == id).ToList();
            if (msgs.Count == 0)
                return BadRequest("Not found Messages!!");
            return Ok(msgs);
        }
        [HttpGet]
        [Route("GetMessagesByReciver/{id}")]
        public IActionResult GetMessage2(int id)
        {
            var msgs = db.Messages.Include(msg => msg.Sender).Where(x => x.ReciverId == id).ToList();
            if (msgs.Count == 0)
                return BadRequest("Not found Messages!!");
            return Ok(msgs);
        }

    }
}
