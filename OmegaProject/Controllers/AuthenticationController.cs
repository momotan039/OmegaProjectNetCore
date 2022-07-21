using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Linq;

namespace OmegaProject.Controllers
{
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
        public IActionResult Login([FromBody]UserLogInDTO u)
        {
            var user=db.Users.SingleOrDefault(x=>x.Email==u.Email && u.Password==x.Password);

                if (user == null)
                return Unauthorized("This User Not Exist");//401=>UNAUTHENTICATED
            return Ok(jwt.GenerateToken(user.Id + "", user.Role == 1 ? true : false));
        }
        [Authorize]
        [HttpGet]
        [Route("GetUserByToken")]
        public IActionResult GetUser()
        {
            var user = db.Users.SingleOrDefault(x => x.Id == int.Parse(jwt.GetTokenClaims()));
            return Ok(user);
        }

    }
}
