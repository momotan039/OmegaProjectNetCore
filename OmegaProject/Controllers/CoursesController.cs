using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        MyDbContext db;
        public CoursesController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("GetCourses")]
        public IActionResult GetCourses()
        {
            return Ok(db.Courses.ToList());
        }


        [HttpPost]
        [Route("PostCourse")]
        public IActionResult PostCourse([FromBody] Course course)
        {
            var temp = db.Courses.FirstOrDefault(x => x.Name == course.Name);
            if (temp != null)
                return BadRequest("Faild Added ...This Course Exist !!");
            db.Courses.Add(course);
            db.SaveChanges();
            return Ok("Added Successfully");
        }
    }
}
