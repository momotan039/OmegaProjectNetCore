

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        public MyDbContext db;
        private readonly JwtService jwt;
         int totalMessages = 20;
        public MessagesController(MyDbContext _db, JwtService jwt)
        {
            db = _db;
            this.jwt = jwt;
        }
        //cannot remove message due to its will remove in a nother user
        [HttpDelete]
        [Route("DeleteMessage/{id}")]
        public IActionResult DeleteMessage(int id)
        {
            var msg = db.Messages.FirstOrDefault(d => d.Id == id);
            if (msg == null)
                return NotFound("Message Not Found");
            db.Messages.Remove(msg);
            db.SaveChanges();
            return StatusCode(200);
        }

        [HttpPost]
        [Route("SendMessage")]
        public IActionResult SendMessage([FromBody] Message msg)
        {
            msg.SendingDate = System.DateTime.Now;
            db.Messages.Add(msg);
            db.SaveChanges();
            return StatusCode(200);
        }




        [HttpPut]
        [Route("ChangeStatusMessage")]
        public IActionResult ChangeStatusMessage([FromBody] Message msg)
        {
            var temp = db.Messages.Find(msg.Id);
            if (temp == null)
                return BadRequest("Not found Message");
            temp.IsOpened = true;
            db.SaveChanges();
            return Ok("Message Changed Status Succsessfully");
        }


        //[HttpGet]
        //[Route("GetMessagesBySender/{idReciver}")]
        //public IActionResult GetMessagesBySender(int idReciver)
        //{
        //    int id = int.Parse(jwt.GetTokenClaims());
        //    var user = db.Users.FirstOrDefault(d => d.Id == id);
        //    List<Message> msgs = null;
        //    if (user.RoleId == 1)
        //        msgs = db
        //            .Messages
        //            .OrderByDescending(d => d.SendingDate)
        //            .Where(d => d.ReciverId == idReciver)
        //            .ToList();
        //    else
        //        msgs = db.Messages.Include(msg => msg.Reciver)
        //            .OrderByDescending(d => d.SendingDate)
        //            .Where(msg => msg.SenderId == id).ToList();

        //    return Ok(msgs);

        //}

        [HttpGet]
        [Route("GetMessagesByReciver/{idReciver}/{current_messages}")]
        public async Task<IActionResult> GetMessagesByReciverAsync(int idReciver,int current_messages)
        {
            int id = int.Parse(jwt.GetTokenClaims());
            //if this a sender or reciver get all messages between each other
            var msgs = await db.Messages.Include(q => q.Sender)
                .Where(x =>
            (x.ReciverId == idReciver && x.SenderId == id) ||
            (x.ReciverId == id && x.SenderId == idReciver)
            ).ToListAsync();

            if (current_messages == 0)
                current_messages = totalMessages;

            bool found_previous = msgs.SkipLast(current_messages).Count()>0? true:false;

            dynamic eo = new ExpandoObject();
            eo.found_previous = found_previous;
            eo.messages= msgs.TakeLast(totalMessages);
            return Ok(eo);
        }


        [HttpGet]
        [Route("GetMessagesByReciver2/{idReciver}/{current_messages}")]
        public async Task<IActionResult> GetMessagesByReciverAsync2(int idReciver, int current_messages)
        {
            int id = int.Parse(jwt.GetTokenClaims());
            //if this a sender or reciver get all messages between each other
            var msgs = await db.Messages.Include(q => q.Sender)
                .Where(x =>
            (x.ReciverId == idReciver && x.SenderId == id) ||
            (x.ReciverId == id && x.SenderId == idReciver)
            ).ToListAsync();

            if (current_messages == 0)
                current_messages = totalMessages;

            bool found_previous = msgs.SkipLast(current_messages).Count() > 0 ? true : false;
            dynamic eo = new ExpandoObject();
            eo.found_previous = found_previous;

            eo.messages = msgs.TakeLast(totalMessages).GroupBy(f => f.SendingDate.ToShortDateString())
                .Select(f => new
                {
                    date = f.Key
                });
            return Ok(eo);
        }

        [HttpGet]
        [Route("GetPreviousMessages/{idReciver}/{current_messages}")]
        public async Task<IActionResult> GetPreviousMessages(int idReciver,int current_messages)
        {
            int id = int.Parse(jwt.GetTokenClaims());
            //if this a sender or reciver get all messages between each other

            var msgs = await db.Messages.Include(q => q.Sender)
                .Where(x =>
            (x.ReciverId == idReciver && x.SenderId == id) ||
            (x.ReciverId == id && x.SenderId == idReciver)
            ).ToListAsync();
            dynamic eo = new ExpandoObject();
            eo.found_previous = msgs.SkipLast(current_messages +totalMessages).Count() > 0 ? true : false;
            eo.messages = msgs.SkipLast(current_messages).TakeLast(totalMessages);
            return Ok(eo);
        }

        [HttpGet]
        [Route("GetAllUnreadMessages")]
        public IActionResult GetAllUnreadMessages()
        {

            int id = int.Parse(jwt.GetTokenClaims());

            //get all groups of this user
            var UsersGroups =  db.UsersGroups.Include(f=>f.Group).Where(f => f.UserId == id).ToList();

            //get msgs that not user sent to group
            var msgsGropup = db.GroupMessages
                .Include(f=>f.OpendGroupMessages)
                .Where(mg =>
                mg.SenderId!=id 
                //&&
                //UsersGroups.Any(f=>f.GroupId==mg.GroupId)
                )
                .ToList();

            //Get All Opend GroupMessages
            var opendGroupMessages = db.OpendGroupMessages.Where(f => f.UserId == id);

            List<ExpandoObject> temp = new List<ExpandoObject>();

            msgsGropup.ForEach(msg =>
            {
                var foundOpenedMessage = opendGroupMessages.FirstOrDefault(f => f.MessageId == msg.Id && f.UserId == id);

                if (foundOpenedMessage == null)
                {
                    dynamic _msg = new ExpandoObject();
                    _msg.id = msg.Id;
                    _msg.senderId = msg.SenderId;
                    _msg.contents = msg.Contents;
                    _msg.isOpened = false;
                    _msg.reciverId = msg.GroupId;
                    _msg.sendingDate = msg.SendingDate;
                    _msg.isGroup = true;
                    temp.Add(_msg);
                }

            });


            var msgs = db.Messages
                .Where(q => !q.IsOpened && q.ReciverId == id).ToList();

            msgs.ForEach(msg =>
            {
                dynamic _msg = new ExpandoObject();
                _msg.id = msg.Id;
                _msg.senderId = msg.SenderId;
                _msg.contents = msg.Contents;
                _msg.isOpened = false;
                _msg.reciverId = msg.ReciverId;
                _msg.sendingDate = msg.SendingDate;
                _msg.isGroup = false;
                temp.Add(_msg);

            });

            return Ok(temp);
        }

        [HttpGet]
        [Route("GetUnreadMessages/{senderId}")]
        public async Task<IActionResult> GetUnreadMessages(int senderId)
        {
            int id = int.Parse(jwt.GetTokenClaims());
            var msgs = await db.Messages
                .Where(q => !q.IsOpened && q.ReciverId == id && q.SenderId == senderId).ToListAsync();
            return Ok(msgs);
        }

        [HttpGet]
        [Route("ReadMessage/{id}")]
        public async Task<IActionResult> ReadMessageAsync(int id)
        {
            var msg = await db.Messages.FirstOrDefaultAsync(f => f.Id == id);
            msg.IsOpened = true;
            await db.SaveChangesAsync();
            return Ok("message status changed successfully");
        }
    }
}
