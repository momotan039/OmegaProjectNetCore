using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Collections.Generic;
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
        [Route("GetCoursesByUserId/{id}")]
        public IActionResult GetGroupsbyUserId(int id)
        {
            if (db.Users.FirstOrDefault(u => u.Id == id) == null)
                return NotFound("Not Found User");

            var groupsId = new List<int>();
            //get all groups that in
            db.UsersGroups.ToList().ForEach(ug =>
            {
                if(ug.UserId==id)
                    groupsId.Add(ug.GroupId);
            });
            var courses=new List<Course>();
            db.Groups.ToList().ForEach(g =>
            {
                groupsId.ForEach(gId =>
                {
                    if (gId == g.Id)
                        courses.Add(db.Courses.FirstOrDefault(c=>c.Id==g.CourseId));
                });
            });
            return Ok(courses);
        }

        [HttpGet]
        [Route("GetCourses")]
        public IActionResult GetCourses()
        {
            return Ok(db.Courses.ToList());
        }
        [HttpGet]
        [Route("GetCourseById/{id}")]
        public IActionResult GetCourseById(int id)
        {
            var c=db.Courses.FirstOrDefault(u=>u.Id==id);
            if (c == null)
                return NotFound("Not Found Course");
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
            return Ok("Course Added Successfully");
        }

        [HttpDelete]
        [Route("DeleteCourse/{id}")]
        public IActionResult DeleteCourse(int id)
        {
            //check if user Existed
            var temp = db.Courses.FirstOrDefault(x => x.Id == id);
            if (temp == null)
                return NotFound("Faild Deleted ...This Course not Exist !!");

            db.Courses.Remove(temp);
            db.SaveChanges();
            return Ok("Course Deleted Successfully");
        }

        [HttpPut]
        [Route("EditCourse")]
        public IActionResult EditCourse([FromBody] Course course)
        {
            //check if Course Existed
            var temp = db.Courses.FirstOrDefault(x => x.Id == course.Id);
            if (temp == null)
                return NotFound("Faild Editing ...This Course not Exist !!");
            temp=db.Courses.FirstOrDefault(f=>f.Name==course.Name);
            if (temp != null)
                return BadRequest("Faild Editing ...Course with same Name Already Exist !!");
            temp.Name = course.Name;
            db.SaveChanges();
            return Ok("Course Edited Successfully");
        }
    }
}
