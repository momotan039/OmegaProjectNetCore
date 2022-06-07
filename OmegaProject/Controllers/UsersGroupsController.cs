using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.DTO;
using OmegaProject.services;
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
        [Route("AddUserToGroup")]
        public IActionResult AddUser([FromBody] UserGroup uG)
        {
            //Check if User Exist
            var user=db.Users.ToList().FirstOrDefault(u => u.Id == uG.UserId);
            if (user == null)
                return BadRequest("This User Not Exist");
            //Check if Group Exist
            var group = db.Groups.ToList().FirstOrDefault(g => g.Id == uG.GroupId);
            if (group == null)
                return BadRequest("This Group Not Exist");

            //Check if UserGroup Exist
            var temp = db.UsersGroups.ToList().FirstOrDefault(t=>t.UserId==user.Id && t.GroupId==group.Id);
            if (temp != null)
                return NotFound("This User is in Group Already");
            //Add User
            db.UsersGroups.Add(uG);
            db.SaveChanges();
            return Ok("Added Successfully");
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
            return Ok("Editing Successfully");
        }
        [HttpGet]
        [Route("GetUserGroups")]
        public IActionResult GetUsers()
        {
            return Ok(db.UsersGroups.ToList());
        }

        [HttpDelete]
        [Route("DeleteUserGroups/{id}")]
        public IActionResult GetUsers(int id)
        {
            var ug = db.UsersGroups.ToList().FirstOrDefault(u => u.Id == id);
            if (ug == null)
                return BadRequest("This GroupUser Not Exist");
            db.UsersGroups.Remove(ug);
            db.SaveChanges();
            return Ok("Deleting Successfully");
        }

    }
}
