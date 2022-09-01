using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Collections.Generic;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //enable cros
    //[EnableCors("myPolicy")]
    public class UsersController : ControllerBase
    {
        public MyDbContext db;
        private readonly JwtService jwtService;

        public UsersController(MyDbContext _db, JwtService jwtService)
        {
            db = _db;
            this.jwtService = jwtService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = MyDbContext.getInctence().Users.Include(u => u.Messages).Include(u=>u.Role).ToList();
            users.Reverse();
            return Ok(users);
        }


        //[Authorize]
        //[HttpGet]
        //[Route("GetUserByToken")]
        //public IActionResult GetUser()
        //{
        //    var user = db.Users.SingleOrDefault(x => x.Id == int.Parse(jwt.GetTokenClaims()));
        //    return Ok(user);
        //}

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("GetUsersByRole/{role}")]
        public IActionResult GetUsersByRole(int role)
        {

            var users = db.Users.Where(u => u.RoleId == role).ToList();
            
            return Ok(users);
        }

        [HttpGet]
        [Route("GetUsersByGroupId/{groupId}")]
        public IActionResult GetUsersByGroupId(int groupId)
        {
            var users=new List<User>();
           
                db.UsersGroups.Include(ug=>ug.User).ThenInclude(q=>q.Role).
                Where(ug => ug.GroupId == groupId).ToList().ForEach(ug =>
                {
                    users.Add(ug.User);
                });

            return Ok(users);
        }
        

        [HttpGet]
        [Route("GetFreindsByUser")]
        public IActionResult GetUsersByGroup()
        {
            var usersRes = new List<User>();
            var usersId=new List<int>();
            int id = int.Parse(jwtService.GetTokenClaims());
            var user=db.Users.SingleOrDefault(ug=>ug.Id == id);

            if (user.RoleId==1)
                return Ok(db.Users.Where(u=>u.Id!=id && u.RoleId!=3).ToList());
           
            //get all groups that referenc to user
            var ugs = db.UsersGroups.Where(g => g.UserId == id).ToList();
            //get usres id that in same group user

            db.UsersGroups.Where(ug=>ug.UserId!=id).ToList().ForEach(ug =>
            {
                ugs.ForEach(g =>
                {
                    if(g.GroupId==ug.GroupId && !usersId.Contains(ug.UserId))
                        usersId.Add(ug.UserId);
                });
            });
            usersId.ForEach(id =>
            {
                usersRes.Add(db.Users.FirstOrDefault(f=>f.Id==id));
            });
            if (user.RoleId == 2)//get admins if user teacher
                db.Users.Where(u => u.RoleId == 1).ToList().ForEach((u) => {
                    usersRes.Add(u);
                });
            return Ok(usersRes);
        }

        [HttpGet]
        [Route("GetUsersNotInThisGroup/{groupId}")]
        public IActionResult GetUsersNotInThisGroup(int groupId)
        {
            var users = new List<User>();
            var currentUsers = new List<User>();
            //get users in this group
            db.UsersGroups.Include(ug => ug.User).Where(ug => ug.GroupId == groupId).ToList().ForEach(ug =>
            {
                currentUsers.Add(ug.User);
            });

            db.Users.Where(u=>u.RoleId!=1).ToList().ForEach(u =>
            {
                //check if user is not in current group
                if (!currentUsers.Contains(u))
                    users.Add(u);
            });
            return Ok(users);
        }

        [HttpPost]
        [Route("PostUser")]
        public IActionResult PostUsers([FromBody] User user)
        {
            var temp=db.Users.FirstOrDefault(x => x.IdCard==user.IdCard || x.Email==user.Email);
            
            if(temp!=null)
                return BadRequest("The Id Card or Email associated with an existing user !!");

            user.Password=MyTools.GenerateHashedPassword();

            db.Users.Add(user);
            db.SaveChanges();
            int id = db.Users.FirstOrDefault(f => f.IdCard == user.IdCard).Id;

            bool successSendMail = MyTools.SendConfirmRegistration(id, user.Email);

            //if (!successSendMail)
            //    return BadRequest("Error while sending Email Confirmation!!,Faild Adding User");

            return Ok("User Added successfully");
        }

        [HttpDelete]
        [Route("DeleteUser/{id}")]
        public IActionResult DeleteUser(int id)
        {
            //check if user Existed
            var temp = db.Users.FirstOrDefault(x => x.Id==id);
            if (temp == null)
                return NotFound("Faild Deleted ...This User not Exist !!");
            db.Users.Remove(temp);
            db.SaveChanges();
            return Ok("User Deleted successfully");
        }

        [HttpPut]
        [Route("EditUser")]
        public IActionResult EditUser([FromBody] User user)
        {
            //check if user Existed
            var temp = db.Users.FirstOrDefault(x => x.Id == user.Id);
            if (temp == null)
                return NotFound("Faild Editing ...This User not Exist !!");
            temp.RoleId=user.RoleId;
            temp.FirstName = user.FirstName;
            temp.LastName = user.LastName;
            temp.Email = user.Email;
            temp.Password = user.Password;
            temp.Phone = user.Phone;
            temp.IdCard = user.IdCard;
            db.SaveChanges();
            return Ok("User Edited successfully");
        }

    }
}
