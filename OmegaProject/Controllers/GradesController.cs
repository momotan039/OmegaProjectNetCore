using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.services;
using System.Collections.Generic;
using System.Dynamic;
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
            var grades = db.Grades.Include(f => f.Group).
                Include(f=>f.Student).
                Include(f=>f.Test).
                ToList();
            return Ok(grades);
        }

        [HttpGet]
        [Route("GetGradesByTest/{testId}")]
        public IActionResult GetGrades(int testId)
        {
            var grades = db.Grades.
                Include(f => f.Test).
                Include(f=>f.Student).
                Include(f=>f.Group).
                Where(f => f.Test.Id == testId).
                ToList();
            List<dynamic> objs = new List<dynamic>();
            grades.ForEach(f =>
            {
                dynamic obj = new ExpandoObject();
                obj.student = new { idCard = f.Student.IdCard };
                obj.group = new {name=f.Group.Name};
                obj.sumGrade = f.SumGrade;
                objs.Add(obj);
            });
            
            return Ok(objs);
            //return ok(grades);
        }


        [HttpPost]
        [Route("SendGrade")]
        public IActionResult SendGrade([FromBody] Grade g)
        {
            if (g == null)
                return BadRequest("Faild saving Grade");

            var xg = db.Grades.FirstOrDefault(
                f => f.GroupId == g.GroupId
            && f.StudentId == g.StudentId
            && f.TestId == g.TestId
            );

            if (xg != null)
                return BadRequest("Current Grade Already Exist For This Student");

            db.Grades.Add(g);
            db.SaveChanges();
            return Ok("Grade Added Successfully");    
        }


        [HttpDelete]
        [Route("DeleteGrade/{id}")]
        public IActionResult DeleteGrade(int id)
        {
            var g=db.Grades.First(f => f.Id == id);
            if(g==null)
                return NotFound("Not Found Grade");
            db.Grades.Remove(g);
            db.SaveChanges();
            return Ok("Grade Deleted Successfully");
        }

        [HttpPut]
        [Route("EditGrade")]
        public IActionResult EditGrades([FromBody] Grade g)
        {
            var grade = db.Grades.FirstOrDefault(e => e.Id == g.Id);

            if(grade==null)
                return BadRequest("Faild Editing");

            grade.SumGrade = g.SumGrade;
            grade.StudentId = g.StudentId;
            grade.GroupId= g.GroupId;
            grade.Note=g.Note;
            grade.TestId = g.TestId;

            var xg = db.Grades.Include(f=>f.Student).FirstOrDefault(
                 f => f.GroupId == g.GroupId
             && f.StudentId == g.StudentId
             && f.TestId == g.TestId
             );

            if (xg!=null && xg.Id != grade.Id)
                return BadRequest("Current Grade Already Exist For This Student");

            db.SaveChanges();
            return Ok("Grade Edited Successfully");
        }
    }
}
