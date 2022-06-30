using Microsoft.AspNetCore.Cors;
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
    //enable cros
    [EnableCors("myPolicy")]
    public class UsersController : ControllerBase
    {
        public MyDbContext db;
        public UsersController(MyDbContext _db)
        {
            db = _db;
        }
         private List<MessageDTO> MyFun(List<Message> msgs)
        {
            var _msgs=new List<MessageDTO>();
            foreach (var msg in msgs)
            {
                _msgs.Add(new MessageDTO
                {
                    Contents = msg.Contents,
                    Id = msg.Id,
                    IsOpened = msg.IsOpened,
                    ReciverId = msg.ReciverId,
                    SenderId = msg.SenderId,
                    Title = msg.Title,
                    Sender=msg.Sender,
                });
            }
            return _msgs;
        }
        [HttpGet]
        [Route("GetUsers/{id?}")]
        public IActionResult GetUsers(int id=-1)
        {
            if (id == -1)
            {
                var users = db.Users.Include(u => u.Messages).ToList();
                users.Reverse();
                return Ok(users);
            }
            User user = db.Users.FirstOrDefault(x => x.Id == id);
            if (user == null)
                return BadRequest("Not found User");

            return Ok(user);
        }
   


        [HttpGet]
        [Route("GetUsersByRole/{role}")]
        public IActionResult GetUsersByRole(int role)
        {

            var users = db.Users.Where(u => u.Role == role).ToList();
            
            return Ok(users);
        }

        [HttpGet]
        [Route("GetUsersByGroupId/{groupId}")]
        public IActionResult GetUsersByGroupId(int groupId)
        {
            var users=new List<User>();
           
                db.UsersGroups.Include(ug=>ug.User).Where(ug => ug.GroupId == groupId).ToList().ForEach(ug =>
                {
                    users.Add(ug.User);
                });

            return Ok(users);
        }


        [HttpGet]
        [Route("GetFreindsByUser/{id}")]
        public IActionResult GetUsersByGroup(int id)
        {
            var usersRes = new List<User>();
            var usersId=new List<int>();    
            //get all groups that referenc to user
            var ugs = db.UsersGroups.Where(g => g.UserId == id).ToList();
            //28 29 
            //get usres id that in same group user
            db.UsersGroups.Where(ug=>ug.UserId!=id).ToList().ForEach(ug =>
            {
                ugs.ForEach(g =>
                {
                    if(g.GroupId==ug.GroupId)
                        usersId.Add(ug.UserId);
                });
            });
            usersId.ForEach(id =>
            {
                usersRes.Add(db.Users.FirstOrDefault(f=>f.Id==id));
            });
            return Ok(usersRes);
        }

        [HttpGet]
        [Route("GetUsersNotInThisGroup/{groupId}")]
        public IActionResult GetUsersNotInThisGroup(int groupId)
        {
            var users = new List<User>();
            var _users = new List<User>();
            //get users in this group
            db.UsersGroups.Include(ug => ug.User).Where(ug => ug.GroupId == groupId).ToList().ForEach(ug =>
            {
                _users.Add(ug.User);
            });

            db.Users.Where(u=>u.Role!=1).ToList().ForEach(u =>
            {
                //check if user is not in current group
                if (!_users.Contains(u))
                    users.Add(u);
            });
            return Ok(users);
        }

        [HttpPost]
        [Route("PostUser")]
        public IActionResult GetUsers([FromBody] User user)
        {
            var temp=db.Users.FirstOrDefault(x => x.IdCard==user.IdCard);
            if(temp!=null)
                return BadRequest("Faild Added ...This User Exist !!");
            db.Users.Add(user);
            db.SaveChanges();
            return Ok("Added Successfully");
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public IActionResult DeleteUser(string id)
        {
            //check if user Existed
            var temp = db.Users.FirstOrDefault(x => x.IdCard ==id);
            if (temp == null)
                return BadRequest("Faild Deleted ...This User not Exist !!");

            db.Users.Remove(temp);
            db.SaveChanges();
            return Ok("Deleted Successfully");
        }

        [HttpPut]
        [Route("EditUser")]
        public IActionResult EditUser([FromBody] User user)
        {
            //check if user Existed
            var temp = db.Users.FirstOrDefault(x => x.Id == user.Id);
            if (temp == null)
                return BadRequest("Faild Editing ...This User not Exist !!");
            temp.Role = user.Role;
            temp.FirstName = user.FirstName;
            temp.LastName = user.LastName;
            temp.Email = user.Email;
            temp.Password = user.Password;
            temp.Phone = user.Phone;
            temp.IdCard = user.IdCard;
            db.SaveChanges();
            return Ok("Editing Successfully");
        }

    }
}
