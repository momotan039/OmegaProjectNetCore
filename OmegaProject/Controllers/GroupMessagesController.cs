using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.Entity;
using OmegaProject.services;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class GroupMessagesController : ControllerBase
    {
        private readonly MyDbContext db;
        private readonly JwtService jwt;
        int totalMessages = 20;
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
        [Route("GetMessagesByReciver/{groupId}/{current_messages}")]
        public IActionResult GetMessagesByReciver(int groupId, int current_messages)
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

            if (current_messages == 0)
                current_messages = totalMessages;

            bool found_previous = msgs.SkipLast(current_messages).Count() > 0 ? true : false;

            dynamic eo = new ExpandoObject();
            eo.found_previous = found_previous;
            eo.messages = msgs.TakeLast(totalMessages);
            return Ok(eo);

            //return Ok(msgs);
        }

        [HttpGet]
        [Route("GetPreviousMessages/{groupId}/{current_messages}")]
        public async Task<IActionResult> GetPreviousMessages(int groupId, int current_messages)
        {
            int id = int.Parse(jwt.GetTokenClaims());
            //if this a sender or reciver get all messages between each other

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

            dynamic eo = new ExpandoObject();
            eo.found_previous = msgs.SkipLast(current_messages + totalMessages).Count() > 0 ? true : false;
            eo.messages = msgs.SkipLast(current_messages).TakeLast(totalMessages);
            return Ok(eo);
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
