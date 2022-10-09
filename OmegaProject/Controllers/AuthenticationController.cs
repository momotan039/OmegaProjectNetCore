using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaProject.Controllers
{
    [EnableCors("myPolicy")]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly MyDbContext db;
        private readonly JwtService jwt;

        public AuthenticationController(MyDbContext db,JwtService jwt)
        {
            this.db = db;
            this.jwt = jwt;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody]UserLogInDTO u)
        {
            string storedPassword=MyTools.CreateHashedPassword(u.Password);
            var user =await db.Users.FirstOrDefaultAsync(x => x.Email==u.Email);

            if (user == null)
                return NotFound("This User Not Exist");

            if (user.Password !=storedPassword)
                return NotFound("Entered Wrong Password!!");

            //Response.Cookies.Append("token", 
            //    jwt.GenerateToken(user.Id + "", user.RoleId == 1 ? true : false,null),

            //    new CookieOptions
            //    {
            //        HttpOnly = true,
            //    }
            //    );
            return Ok(jwt.GenerateToken(user.Id + "", user.RoleId == 1 ? true : false,null));
            //return Ok("Welcome Back Sir...");
        }
        [Authorize]
        [HttpGet]
        [Route("GetUserByToken")]
        public IActionResult GetUser()
        {
            var user = db.Users.Include(f=>f.Role).
                SingleOrDefault(x => x.Id == int.Parse(jwt.GetTokenClaims()));
            return Ok(user);
        }

    }
}
