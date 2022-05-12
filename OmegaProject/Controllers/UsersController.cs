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
            return Ok(db.Users);

            UserDTO user=db.Users.FirstOrDefault(x => x.Id==id);
            if (user == null)
                return BadRequest("Not found User");

            return Ok(user);
        }


    }
}
