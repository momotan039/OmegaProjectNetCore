using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.DTO;
using OmegaProject.Entity;
using OmegaProject.services;
using System.Collections.Generic;
using System.IO;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeWorkController : ControllerBase
    {
        MyDbContext db;
        private readonly IHostingEnvironment hosting;

        public HomeWorkController(MyDbContext db,IHostingEnvironment hosting)
        {
            this.db = db;
            this.hosting = hosting;
        }

        [HttpPost]
        [Route("SendHomeWork")]
        public IActionResult SendHomeWork([FromBody] HomeWorkDTO homeWork)
        {
            //if (ff == null)
            //    return BadRequest("Did not recive a picture!!");
            
            string path = Path.Combine(hosting.WebRootPath,"HomeWork",
                "Images",homeWork.FilesPath.FileName);
            homeWork.FilesPath.CopyTo(new FileStream(path,FileMode.Create));
            return Ok("File Uploaded Successfully!!");
        }
    }
}
