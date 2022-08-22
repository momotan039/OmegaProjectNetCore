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
            var user=db.Users.FirstOrDefault(q=>q.Email==model.Email);
            if(user==null)
                return NotFound("User Not Exist!!");
            user.Password = MyTools.CreateHashedPassword(model.Password);
            db.SaveChanges();
            return Ok("Password Changed Successfully!");
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public IActionResult ForgetPassword([FromBody]string mail)
        {

            var user = db.Users.FirstOrDefault(q => q.Email == mail);

            if (user == null)
                return NotFound("User Not Exist!!");

            string token=jwt.GenerateToken(mail, false);
            token = token.Replace("Bearer ", "");
            bool success = MyTools.SendResetPassMail(token, mail);
            if(!success)
            return BadRequest("Occured Error While Sending Link to Mail!!");

            return Ok("Link Sended to Mail Successfully!");
        }
    }
}
