using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public object BuilderString { get; private set; }

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
            if (files.Length !=0)
            {
                Init_Group_Teacher_Folders(path, homeWork);
                string mainRoot = Path.Combine(hosting.WebRootPath, "HomeWork",
                    "Files", $"{homeWork.GroupId}", $"{homeWork.TeacherId}");
                foreach (var file in files)
                {
                    path = CustomizeNameFile(mainRoot, file.FileName);
                    homeWork.FilesPath += path+"\n";
                }
                // if user Uploaded files .. save it
                var paths = homeWork.FilesPath.Split('\n').ToArray();
                try
                {
                    SaveFileOnServerStorage(paths, files);
                }
                catch (Exception r)
                { 
                    return BadRequest("Occured Error While Saving...Try Again");
                }
            }

            //save homework to database
            db.HomeWorks.Add(homeWork);
            db.SaveChanges();
            return Ok();

        }

        private void SaveFileOnServerStorage(string[] paths, IFormFile[] files)
        {
            int index = 0;
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

        private string CustomizeNameFile(string mainRoot,string file)
        {//x/x/x/file.png

            if (!System.IO.File.Exists(Path.Combine(mainRoot, file)))
                return Path.Combine(mainRoot, file);
            string ext = Path.GetExtension(file);
            int i = 1;
            while (true)
            {
                file = Path.GetFileNameWithoutExtension(file);//f.text
                if (i == 1)
                    file += "_1";
                else
                    file = file.Replace($"_{i - 1}", $"_{i}");
                file += ext;
                if (!System.IO.File.Exists(Path.Combine(mainRoot, file)))
                    return Path.Combine(mainRoot, file);
                i++;
            }
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
            return Ok(db.HomeWorks.Include(q=>q.Teacher).Include(q => q.Group).FirstOrDefault(f => f.Id == id));
        }
        [HttpGet]
        [Route("GetHomeWorkByTeacherId/{id}")]
        public IActionResult GetHomeWorkByTeacherId(int id)
        {
            return Ok(db.HomeWorks.Where(w => w.TeacherId == id).ToList());
        }

        [HttpGet]
        [Route("GetHomeWorkByStudentId/{id}")]
        public IActionResult GetHomeWorkByStudentId(int id)
        {
            //get all gorups that current student is in
            var ugs=db.UsersGroups.Where(q=>q.UserId==id).ToList();
            var homeworks=new  List<HomeWork>();
            ugs.ForEach(f =>
            {
                homeworks.AddRange(db.HomeWorks.Include(q=>q.Group).Include(q=>q.Teacher)
                    .Where(q => q.GroupId == f.GroupId));
            });
            homeworks = homeworks.OrderBy(q => q.SendingDate).Reverse().ToList();
            return Ok(homeworks);
        }

        [HttpGet]
        [Route("DownloadHomeWorkFile")]
        public ActionResult DownloadDocument(int GroupId, int TeacherId, string name)
        {
            string url = hosting.WebRootPath 
                + "/HomeWork/Files/" +
                GroupId + "/" +
                TeacherId +"/" +
                name;

            if(!System.IO.File.Exists(url))
                return NotFound("Not Found File");

            byte[] fileBytes = System.IO.File.ReadAllBytes(url);
            return File(fileBytes, "application/force-download", name);
        }
    }
   
}
