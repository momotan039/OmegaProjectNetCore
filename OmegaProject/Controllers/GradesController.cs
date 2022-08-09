using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
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

        [HttpPost]
        [Route("SendGrade")]
        public IActionResult SendGrade([FromBody] Grade g)
        {
            if (g == null)
                return BadRequest("Faild saving Grade");
            db.Grades.Add(g);
            db.SaveChanges();
            return Ok();    
        }

        [HttpDelete]
        [Route("DeleteGrade")]
        public IActionResult DeleteGrade([FromBody] Grade g)
        {
            db.Grades.Remove(g);
            db.SaveChanges();
            return Ok();
        }

        [HttpPut]
        [Route("EditGrade")]
        public IActionResult EditGrades([FromBody] Grade g)
        {
            var grade = db.Grades.FirstOrDefault(e => e.Id == g.Id);

            if(grade==null)
                return NotFound();

            grade.SumGrade = g.SumGrade;
            grade.StudentId = g.StudentId;
            grade.GroupID= g.GroupID;
            grade.Note=g.Note;
            return Ok(grade);
        }
    }
}
