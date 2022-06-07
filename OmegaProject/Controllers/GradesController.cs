using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.services;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradesController : ControllerBase
    {
        MyDbContext db;
        public GradesController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("GetGrades")]
        public IActionResult GetGrades()
        {
            var grades = db.Grades.Include(f => f.Group).Include(f=>f.Student).ToList();
            return Ok(grades);
        }
    }
}
