using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.Entity.Guest_Site;
using OmegaProject.services;
using System;
using System.Linq;

namespace OmegaProject.Controllers.Guest_Site
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class OpinionsController : ControllerBase
    {
        private readonly MyDbContext db;

        public OpinionsController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {

            var data = db.Opinions.OrderBy(g=>Guid.NewGuid()).Take(4);
            return Ok(data);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Post([FromBody] Opinion o)
        {
            try
            {
                db.Opinions.Add(o);
                db.SaveChanges();
            }
            catch
            {
                return BadRequest("Error While Adding Opinion");
            }

            return Ok("Opinion saving Sucessfully");

        }

        [HttpPut]
        [Route("EditOne")]
        public IActionResult EditOne([FromBody] Opinion o)
        {
          
            try
            {
                var _o = db.Opinions.FirstOrDefault(f => f.Id == o.Id);
                if (_o == null)
                    return NotFound("Not Found This Opinion");
                _o.About = o.About;
                _o.Name = o.Name;
                _o.Content = o.Content;
                db.SaveChanges();
            }
            catch
            {
                return BadRequest("Error While Adding Opinion");
            }

            return Ok("Opinion Editing Sucessfully");

        }


        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {

            try
            {
                var _o = db.Opinions.FirstOrDefault(f => f.Id == id);
                if (_o == null)
                    return NotFound("Not Found This Opinion");
                db.Opinions.Remove(_o);
                db.SaveChanges();
            }
            catch
            {
                return BadRequest("Error While Deleting Opinion");
            }

            return Ok("Opinion Deleting Sucessfully");

        }
    }
}
