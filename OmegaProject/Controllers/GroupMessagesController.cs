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
        [Route("GetMessagesByReciver/{idReciver}")]
        public IActionResult GetMessagesByReciver(int idReciver)
        {
            int id = int.Parse(jwt.GetTokenClaims());
            var msgs = db.GroupMessages.Include(q => q.Sender).Where(x =>
              (x.GroupId == idReciver)).ToList();
            //msgs.Reverse();
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
    }
}
