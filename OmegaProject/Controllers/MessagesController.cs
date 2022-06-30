

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
        //cannot remove message due to its will remove in a nother user
        //[HttpDelete]
        //[Route("DeleteMessage/{id}")]
        //public IActionResult DeleteMessage(int id)
        //{
        //    var msg = db.Messages.FirstOrDefault(d=>d.Id==id);
        //    if (msg == null)
        //        return BadRequest("Message Not Found");
        //    db.Messages.Remove(msg);
        //    return Ok("Message Deleted Successfully");
        //}

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
                  //datetime without seconds
                  var dt2 = System.DateTime.Now;
                  var dt = new System.DateTime(dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, 0);

                  var m = new Message()
                  {
                      Contents = msg.Contents,
                      ReciverId=u.User.Id,
                      SenderId=msg.SenderId,
                      Title = msg.Title,
                      SendingDate = dt,
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
            var user = db.Users.FirstOrDefault(d => d.Id == id);
            List<Message> msgs = null;
            if (user.Role == 1)
             
                msgs = db.Messages
                    //GroupBy(x => new { x.SenderId, x.Title, x.Contents, x.SendingDate })
                    .GroupBy(x => new { x.Title,x.Contents,x.SenderId,x.SendingDate })
                    .Select(r => new Message
                    {
                        SenderId=r.Key.SenderId,
                        Contents=r.Key.Contents,
                        Title=r.Key.Title,
                        SendingDate=r.Key.SendingDate,
                    }).ToList();
            else
                msgs = db.Messages.Include(msg=>msg.Reciver).Where(msg => msg.SenderId == id).ToList();
            if (msgs.Count == 0)
                return BadRequest("Not found Messages!!");

            msgs.Reverse();
            return Ok(msgs);

        }
        [HttpGet]
        [Route("GetMessagesByReciver/{id}")]
        public IActionResult GetMessagesByReciver(int id)
        {
            var msgs = db.Messages.Include(msg => msg.Sender).Where(x => x.ReciverId == id).ToList();
            
            if (msgs.Count == 0)
                return BadRequest("Not found Messages!!");
            msgs.Reverse();
            return Ok(msgs);
        }

    }
}
