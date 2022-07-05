using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.DTO;
using OmegaProject.Entity;
using OmegaProject.services;
using System;
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
        public IActionResult SendHomeWork(IFormFile[] files)
        {
            string path = Path.Combine(hosting.WebRootPath, "HomeWork",
                   "Files");

            var homeWork = new HomeWork();
            homeWork.Title = HttpContext.Request.Form["title"];
            homeWork.Contents = HttpContext.Request.Form["contents"];
            homeWork.GroupId = int.Parse(HttpContext.Request.Form["groupId"]);
            homeWork.TeacherId = int.Parse(HttpContext.Request.Form["teacherId"]);
            homeWork.SendingDate = System.DateTime.Now;

            //get paths of uploaded files
            if (files != null)
            {
                Init_Group_Teacher_Folders(path, homeWork);
                foreach (var file in files)
                {
                    path = Path.Combine(hosting.WebRootPath, "HomeWork",
                    "Files", $"{homeWork.GroupId}", $"{homeWork.TeacherId}", file.FileName);
                    homeWork.FilesPath += path+"\n";
                }
            }

            // if user Uploaded files save it
            var paths = homeWork.FilesPath.Split('\n').ToArray();
            int index = 0;
            try
            {
                foreach (var file in files)
                {
                    using (var fs = new FileStream(paths[index++], FileMode.Create))
                    {
                        if (file != null)
                        {
                            file.CopyTo(fs);
                        }
                    }
                }
            }
            catch (Exception r)
            {
                return BadRequest(r.Message);
            }

            //save homework to database
            db.HomeWorks.Add(homeWork);
            db.SaveChanges();
            return Ok("HomeWorkSaved Successfully Successfully!!");
        }

        private void Init_Group_Teacher_Folders(string path, HomeWork homeWork)
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
