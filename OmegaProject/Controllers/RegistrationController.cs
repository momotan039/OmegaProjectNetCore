using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly MyDbContext db;

        public RegistrationController(MyDbContext db)
        {
            this.db = db;
        }
        [HttpPost]
        [Route("ConfirmRegistration")]
        public IActionResult ConfirmRegistration(RegistrationModel model)
        {
            var user=db.Users.FirstOrDefault(f => f.Id == model.Id);

            if(user==null)
                return NotFound("User Not Exist!!");

            user.Password=MyTools.CreateHashedPassword(model.Password);
            user.ConfirmPassword = true;
            db.SaveChanges();
            return Ok("Confirmed Successfully , Please Login In to Complete");
        }

        [HttpGet]
        [Route("GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = db.Users.Include(q => q.Role).SingleOrDefault(u => u.Id == id);
            return Ok(user);
        }
    }
}
