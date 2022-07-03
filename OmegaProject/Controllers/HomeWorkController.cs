using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.DTO;
using OmegaProject.Entity;
using OmegaProject.services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeWorkController : ControllerBase
    {
        private readonly MyDbContext db;
        private readonly IHostingEnvironment hosting;

        public HomeWorkController(MyDbContext db,IHostingEnvironment hosting)
        {
            this.db = db;
            this.hosting = hosting;
        }


        [HttpPost]
        [Route("SendHomeWork")]
        public IActionResult SendHomeWork(IFormFile files)
        {
            //get uploded files path
            string path = Path.Combine(hosting.WebRootPath, "HomeWork",
                    "Images", files.FileName);

            var homeWork=new HomeWork();    
            homeWork.Title=HttpContext.Request.Form["title"];
           homeWork.Contents=HttpContext.Request.Form["contents"];
            homeWork.GroupId=int.Parse(HttpContext.Request.Form["groupId"]);
            homeWork.SendingDate = System.DateTime.Now;
            homeWork.FilesPath=path;

            //save homework to database
            db.HomeWorks.Add(homeWork);
            db.SaveChanges();

            //save uploaded files to server 
            if (files != null)
            {
                files.CopyTo(new FileStream(path, FileMode.Create));
            }

            return Ok("HomeWorkSaved Successfully Successfully!!");
        }

        [HttpGet]
        [Route("GetHomeWork/{id?}")]
        public IActionResult GetHomeWork(int id=-1)
        {
            if (id == -1)
                return Ok(db.HomeWorks.ToList());
            return Ok(db.HomeWorks.FirstOrDefault(f=>f.Id==id));
        }
    }
}
