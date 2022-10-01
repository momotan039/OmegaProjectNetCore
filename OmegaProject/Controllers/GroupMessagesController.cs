using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.Entity;
using OmegaProject.services;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupMessagesController : ControllerBase
    {
        private readonly MyDbContext db;
        private readonly JwtService jwt;

        public GroupMessagesController(MyDbContext _db, JwtService jwt)
        {
            db = _db;
            this.jwt = jwt;
        }

        [HttpPost]
        [Route("SendMessage")]
        public IActionResult SendMessage([FromBody] GroupMessage msg)
        {
            msg.SendingDate = System.DateTime.Now;
            db.GroupMessages.Add(msg);
            db.SaveChanges();
            return StatusCode(200);
        }
        [HttpGet]
        [Route("GetMessagesByReciver/{groupId}")]
        public IActionResult GetMessagesByReciver(int groupId)
        {
            int id = int.Parse(jwt.GetTokenClaims());

            var msgs = db.GroupMessages.Include(q => q.Sender).Where(x =>
              (x.GroupId == groupId)).Include(f => f.OpendGroupMessages).ToList();

            //Get All Opend GroupMessages
            var opendGroupMessages = db.OpendGroupMessages.Where(f => f.UserId == id);


            msgs.ForEach(msg =>
            {
                var foundOpenedMessage = opendGroupMessages.FirstOrDefault(f => f.MessageId == msg.Id && f.UserId == id);
                if (foundOpenedMessage != null)
                    msg.IsOpened = true;
            });

            return Ok(msgs);
        }
        [HttpDelete]
        [Route("DeleteMessage/{id}")]
        public IActionResult DeleteMessage(int id)
        {
            var msg = db.GroupMessages.FirstOrDefault(d => d.Id == id);
            if (msg == null)
                return NotFound("Message Not Found");
            db.GroupMessages.Remove(msg);
            db.SaveChanges();
            return StatusCode(200);
        }

        [HttpGet]
        [Route("ReadMessage/{id}/{userId}")]
        public IActionResult ReadMessageAsync(int id, int userId)
        {
            var msg = db.OpendGroupMessages.FirstOrDefault(f => f.UserId == userId && id == f.MessageId);
            if (msg != null)
                return Ok();

            msg = new OpendGroupMessage { UserId = userId, MessageId = id };
            db.OpendGroupMessages.Add(msg);
            db.SaveChanges();
            return Ok("message status changed successfully");
        }

        [HttpGet]
        [Route("GetAllOpendGroupMessages")]
        public IActionResult GetAllOpendGroupMessages()
        {
            return Ok(db.OpendGroupMessages);
        }
    }
}
