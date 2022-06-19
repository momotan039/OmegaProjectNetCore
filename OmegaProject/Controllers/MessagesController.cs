

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Collections.Generic;
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
            msg.SendingDate = System.DateTime.Now;
            db.Add(msg);
            db.SaveChanges();
            return  Ok("Message Sent Succsessfully");
        }

        [HttpPost]
        [Route("SendMessageByGroup")]
        public IActionResult SendMessageToGroup([FromBody] MessageGroupDTO msg)
        {
            //get id group 
            //get all users in current group
            //send msg to these users
            int gId = msg.GroupId;
            db.UsersGroups.Include(ug => ug.User).Where(u => u.GroupId == gId).ToList().ForEach(u =>
              {
                  var m = new Message()
                  {
                      Contents = msg.Contents,
                      ReciverId=u.User.Id,
                      SenderId=msg.SenderId,
                      Title = msg.Title,
                      SendingDate = System.DateTime.Now,
                  };
                  db.Add(m);
                  db.SaveChanges();
              });
            
            return Ok("Messages Sent Succsessfully");
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
        public IActionResult GetMessagesBySender(int id)
        {
            var msgs = db.Messages.Where(msg => msg.SenderId == id).ToList();
            if (msgs.Count == 0)
                return BadRequest("Not found Messages!!");
            return Ok(msgs);
        }
        [HttpGet]
        [Route("GetMessagesByReciver/{id}")]
        public IActionResult GetMessagesByReciver(int id)
        {
            var user=db.Users.FirstOrDefault(d=>d.Id==id);
            List<Message> msgs = null;
            if(user.Role==1)
                msgs=db.Messages.Include(msg => msg.Sender).Where(x => x.ReciverId == id).ToList();
            else
             msgs = db.Messages.Include(msg => msg.Sender).Where(x => x.ReciverId == id).ToList();
            if (msgs.Count == 0)
                return BadRequest("Not found Messages!!");
            return Ok(msgs);
        }

    }
}
