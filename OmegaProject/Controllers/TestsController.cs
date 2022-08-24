using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.Entity;
using OmegaProject.services;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestsController : ControllerBase
    {
        private readonly MyDbContext db;

        public TestsController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("GetTests")]
        public IActionResult GetTests()
        {
            return Ok(db.Tests.OrderByDescending(f=>f.Date).ToList());
        }
        [HttpGet]
        [Route("GetTestById/{id}")]
        public IActionResult GetTestById(int id)
        {
            return Ok(db.Tests.FirstOrDefault(q=>q.Id==id));
        }

        [HttpPost]
        [Route("PostTest")]
        public IActionResult PostTest([FromBody] Test test)
        {
            if (test == null)
                return BadRequest("Cannot Add Null Test");
            var t=db.Tests.FirstOrDefault(x => x.Name == test.Name && test.Date.CompareTo(x.Date)==0);
            if (t!=null)
                return BadRequest("This Test Already Exist");

            db.Tests.Add(test);
            db.SaveChanges();
            return Ok("Test Added Successfully");
        }

        [HttpPut]
        [Route("EditTest")]
        public IActionResult EditTest([FromBody] Test test)
        {
            var t = db.Tests.FirstOrDefault(q => q.Id == test.Id);
            if (t == null)
                return NotFound("Not Found Test");

            t.Name=test.Name;
            t.Date=test.Date;
            db.SaveChanges();
            return Ok("Test Edited Successfully");
        }

        [HttpDelete]
        [Route("DeleteTest/{id}")]
        public IActionResult DeleteTest(int id)
        {
            var t = db.Tests.FirstOrDefault(q => q.Id == id);
            if (t == null)
                return NotFound("Not Found Test");

            db.Tests.Remove(t);
            db.SaveChanges();
            return Ok("Test Deleted Successfully");
        }
    }
}
