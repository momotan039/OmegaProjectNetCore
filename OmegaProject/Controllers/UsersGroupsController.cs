using Microsoft.AspNetCore.Http;
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
    public class UsersGroupsController : ControllerBase
    {
        MyDbContext db;
        public UsersGroupsController(MyDbContext db)
        {
            this.db = db;
        }
        [HttpPost]
        [Route("AddUsersToGroup")]
        public IActionResult AddUsers([FromBody] UserGroup [] uGs)
        {
            foreach(var ug in uGs)
            {
                db.UsersGroups.Add(ug);
            }
            //Add User
            db.SaveChanges();
             return Ok("User Added To Group Successfully");
        }

        [HttpPost]
        [Route("AddUserToGroup")]
        public IActionResult AddUser([FromBody] UserGroup uG)
        {
            //Check if User Exist
            var user=db.Users.ToList().FirstOrDefault(u => u.Id == uG.UserId);
            if (user == null)
                return NotFound("User Not Found");

            //Check if Group Exist
            var group = db.Groups.ToList().FirstOrDefault(g => g.Id == uG.GroupId);
            if (group == null)
                return NotFound("Group Not Found");

            //Check if UserGroup Exist
            var temp = db.UsersGroups.ToList().FirstOrDefault(t=>t.UserId==user.Id && t.GroupId==group.Id);
            if (temp != null)
                return NotFound("User Already in Current Group"); 

            //Add User
            db.UsersGroups.Add(uG);
            db.SaveChanges();
            return Ok("User Added To Group Successfully");
        }
        [HttpPut]
        [Route("EditUserToGroup")]
        public IActionResult EditUser([FromBody] UserGroup uG)
        {
            var ug = db.UsersGroups.ToList().FirstOrDefault(u=>u.Id==uG.Id);
            if (ug == null)
                return BadRequest("This GroupUser Not Exist");
            //Edit User
            ug.GroupId = uG.GroupId;
            ug.UserId = uG.UserId;
            db.SaveChanges();
            return Ok("Edited successfully");
        }
        [HttpGet]
        [Route("GetUserGroups/{groupId?}")]
        public IActionResult GetUsers(int groupId=-1)
        {
            if(groupId==-1)
            return Ok(db.UsersGroups.ToList());

            var usersGroups = new List<UserGroup>();
            db.UsersGroups.Include(ug => ug.User).Where(ug => ug.GroupId == groupId).ToList().ForEach(ug =>
            {
                usersGroups.Add(ug);
            });

            return Ok(usersGroups);
        }

        [HttpDelete]
        [Route("DeleteUserFromGroup/{id}")]
        public IActionResult DeleteUserFromGroup(int id)
        {
            var ug = db.UsersGroups.FirstOrDefault(u => u.UserId == id);

            if (ug == null)
                return StatusCode(404);
            db.UsersGroups.Remove(ug);
            db.SaveChanges();
            return Ok("Deleted successfully");
        }

    }
}
