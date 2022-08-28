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
    public class RoleController : ControllerBase
    {
        public MyDbContext db;
        public RoleController(MyDbContext _db)
        {
            db = _db;
        }

        [HttpGet]
        [Route("GetRoles/{id?}")]
        public IActionResult GetRoles(int id = -1)
        {
            if (id == -1)
                return Ok(db.Role);

            Role role = db.Role.FirstOrDefault(x => x.NumberRole == id);
            if (role == null)
                return NotFound("Not found Role");

            return Ok(role);
        }

    }
}
