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

        public HomeWorkController(MyDbContext db, IHostingEnvironment hosting)
        {
            this.db = db;
            this.hosting = hosting;
        }


        [HttpPost]
        [Route("SendHomeWork")]
        public IActionResult SendHomeWork(IFormFile files)
        {
            string path = Path.Combine(hosting.WebRootPath, "HomeWork",
                   "Files");

            var homeWork = new HomeWork();
            homeWork.Title = HttpContext.Request.Form["title"];
            homeWork.Contents = HttpContext.Request.Form["contents"];
            homeWork.GroupId = int.Parse(HttpContext.Request.Form["groupId"]);
            homeWork.TeacherId = int.Parse(HttpContext.Request.Form["teacherId"]);
            homeWork.SendingDate = System.DateTime.Now;

            //get uploded files path
            if (files != null)
            {
                path = Path.Combine(hosting.WebRootPath, "HomeWork",
                    "Files", $"{homeWork.GroupId}", $"{homeWork.TeacherId}", files.FileName);
                homeWork.FilesPath = path;
            }

           
            //save homework to database
            db.HomeWorks.Add(homeWork);
            db.SaveChanges();


            // if user Uploaded files save it
            if (files != null)
            {
                CheckExisted_Group_Teacher_Folders(path, homeWork);
                files.CopyTo(new FileStream(path, FileMode.Create));
            }

            return Ok("HomeWorkSaved Successfully Successfully!!");
        }

        private void CheckExisted_Group_Teacher_Folders(string path, HomeWork homeWork)
        {
            //check if exist Group id folder adn create it
            if (!Directory.Exists(path + $"\\{homeWork.GroupId}"))
                Directory.CreateDirectory(path + $"\\{homeWork.GroupId}");

            //check if exist teacher id and create it
            if (!Directory.Exists(path + $"\\{homeWork.GroupId}" + $"\\{homeWork.TeacherId}"))
                Directory.CreateDirectory(path + $"\\{homeWork.GroupId}" + $"\\{homeWork.TeacherId}");
        }

        [HttpGet]
        [Route("GetHomeWork/{id?}")]
        public IActionResult GetHomeWork(int id = -1)
        {
            //var imagesFolder = Path.Combine(hosting.WebRootPath, "HomeWork",
            //        "Images");
            //foreach (var file in Directory.GetFiles(imagesFolder))
            //{
            //    System.IO.File.Delete(file);
            //}

            //return Ok("Yes");
            if (id == -1)
                return Ok(db.HomeWorks.ToList());
            return Ok(db.HomeWorks.FirstOrDefault(f => f.Id == id));
        }
    }
}
