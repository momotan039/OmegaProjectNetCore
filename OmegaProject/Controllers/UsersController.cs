using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.DTO;
using OmegaProject.services;
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

        [HttpGet]
        [Route("GetUsers/{id?}")]
        public IActionResult GetUsers(int id=-1)
        {
            if(id==-1)
            {
                var users = db.Users.ToList();
                users.Reverse();
                return Ok(users);
            }
            UserDTO user=db.Users.FirstOrDefault(x => x.Id==id);
            if (user == null)
                return BadRequest("Not found User");

            return Ok(user);
        }


        [HttpPost]
        [Route("PostUser")]
        public IActionResult GetUsers([FromBody] UserDTO user)
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
        public IActionResult EditUser([FromBody] UserDTO user)
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
