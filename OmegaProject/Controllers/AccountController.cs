using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly MyDbContext db;
        private readonly JwtService jwt;

        public AccountController(MyDbContext db,JwtService jwt)
        {
            this.db = db;
            this.jwt = jwt;
        }
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(UserLogInDTO model)
        {
            model.Email = jwt.GetTokenClaims();
            var user=db.Users.FirstOrDefault(q=>q.Email==model.Email);
            if(user==null)
                return NotFound("User Not Exist!!");
            user.Password = MyTools.CreateHashedPassword(model.Password);
            db.SaveChanges();
            return Ok("Password Changed Successfully!");
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public IActionResult ForgetPassword([FromBody]UserLogInDTO u)
        {

            var user = db.Users.FirstOrDefault(q => q.Email == u.Email);

            if (user == null)
                return NotFound("User Not Exist!!");

            string token=jwt.GenerateToken(u.Email, false,new System.TimeSpan(0,5,0));

            token = token.Replace("Bearer ", "");

            bool success = MyTools.SendResetPassMail(token, u.Email);

            if(!success)
            return BadRequest("Occured Error While Sending Link to Mail!!");

            return Ok("Link Sended to Mail Successfully!");
        }
    }
}
