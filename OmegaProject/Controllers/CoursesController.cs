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
        [Route("GetCourses/{id?}")]
        public IActionResult GetCourses(int id=-1)
        {
            Course c = null;
            if (id==-1)
            return Ok(db.Courses.ToList());
            else
            {
                c=db.Courses.ToList().FirstOrDefault(f => f.Id == id);
            }
            if (c == null)
                return BadRequest("Course Not Found !!");
            return Ok(c);
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

        [HttpDelete]
        [Route("DeleteCourse/{id}")]
        public IActionResult DeleteCourse(int id)
        {
            //check if user Existed
            var temp = db.Courses.FirstOrDefault(x => x.Id == id);
            if (temp == null)
                return BadRequest("Faild Deleted ...This Course not Exist !!");

            db.Courses.Remove(temp);
            db.SaveChanges();
            return Ok("Deleted Successfully");
        }

        [HttpPut]
        [Route("EditCourse")]
        public IActionResult EditCourse([FromBody] Course course)
        {
            //check if user Existed
            var temp = db.Courses.FirstOrDefault(x => x.Id == course.Id);
            if (temp == null)
                return BadRequest("Faild Editing ...This Course not Exist !!");
           temp.Name = course.Name;
            db.SaveChanges();
            return Ok("Editing Successfully");
        }
    }
}
