﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.Entity;
using OmegaProject.services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
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
                   "Teachers");

            var homeWork = new HomeWork();
            homeWork.Title = HttpContext.Request.Form["title"];
            homeWork.Contents = HttpContext.Request.Form["contents"];
            homeWork.GroupId = int.Parse(HttpContext.Request.Form["groupId"]);
            homeWork.TeacherId = int.Parse(HttpContext.Request.Form["teacherId"]);
            homeWork.RequiredSubmit = bool.Parse(HttpContext.Request.Form["requiredSubmit"]);
            homeWork.SendingDate = System.DateTime.Now;

            db.HomeWorks.Add(homeWork);
            db.SaveChanges();
            //Handel uploded files 
            if (files.Length != 0)
            {
                InitNecessaryFolders(path, homeWork,homeWork.Id);
                string mainRoot = Path.Combine(hosting.WebRootPath, "HomeWork",
                    "Teachers", $"{homeWork.GroupId}", $"{homeWork.TeacherId}",$"{homeWork.Id}");
                foreach (var file in files)
                {
                    path = CustomizeNameFile(mainRoot, file.FileName);
                    homeWork.FilesPath += path + "\n";
                }
                // if user Uploaded files .. save it
                var paths = homeWork.FilesPath.Split('\n').ToArray();
                try
                {
                    SaveFileOnServerStorage(paths, files);
                }
                catch (Exception r)
                {
                    db.HomeWorks.Remove(homeWork);
                    db.SaveChanges();
                    return BadRequest("Occured Error While Saving...Try Again");
                }
            }
            //save last changes => files path
            db.SaveChanges();
            return Ok("Home Work Sended Successfully");

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

        private string CustomizeNameFile(string mainRoot, string file)
        {

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

        private void InitNecessaryFolders(string path, HomeWork homeWork,int id)
        {
            //check if exist Group id folder adn create it
            if (!Directory.Exists(path + $"\\{homeWork.GroupId}"))
                Directory.CreateDirectory(path + $"\\{homeWork.GroupId}");

            //check if exist teacher id and create it
            if (!Directory.Exists(path + $"\\{homeWork.GroupId}" + $"\\{homeWork.TeacherId}"))
                Directory.CreateDirectory(path + $"\\{homeWork.GroupId}" + $"\\{homeWork.TeacherId}");

            //check if exist homework id and create it
            if (!Directory.Exists(path + $"\\{homeWork.GroupId}" + $"\\{homeWork.TeacherId}" + $"\\{id}"))
                Directory.CreateDirectory(path + $"\\{homeWork.GroupId}" + $"\\{homeWork.TeacherId}" + $"\\{id}");

        }

        [HttpPut]
        [Route("EditHomeWork")]

        public IActionResult EditHomeWork(IFormFile[] files)
        {
            string path = Path.Combine(hosting.WebRootPath, "HomeWork",
                  "Teachers");

            int id = int.Parse(HttpContext.Request.Form["id"]);

            var homeWork = db.HomeWorks.FirstOrDefault(f => f.Id == id);
            if (homeWork == null)
                return NotFound("Not Found HomeWork");

            homeWork.Title = HttpContext.Request.Form["title"];
            homeWork.Contents = HttpContext.Request.Form["contents"];
            homeWork.GroupId = int.Parse(HttpContext.Request.Form["groupId"]);
            homeWork.TeacherId = int.Parse(HttpContext.Request.Form["teacherId"]);
            homeWork.RequiredSubmit = bool.Parse(HttpContext.Request.Form["requiredSubmit"]);
            db.SaveChanges();


            //if change Requied Submit to false 
            //Delete all realated files and Submite Homeworks
            db.HomeWorkStudents.RemoveRange(db.HomeWorkStudents.Where(f => f.HomeWorkId == homeWork.Id));
            

            //Handel uploded files 
            if (files.Length != 0)
            {
                InitNecessaryFolders(path, homeWork, homeWork.Id);
                string mainRoot = Path.Combine(path,
                    "Teachers",
                    $"{homeWork.GroupId}",
                    $"{homeWork.TeacherId}",
                    $"{homeWork.Id}");

                foreach (var file in files)
                {
                    path = CustomizeNameFile(mainRoot, file.FileName);
                    homeWork.FilesPath += path + "\n";
                }

                // if user Uploaded files .. save it
                var paths = homeWork.FilesPath.Split('\n').ToArray();
                try
                {
                    SaveFileOnServerStorage(paths, files);
                }
                catch (Exception r)
                {
                    db.HomeWorks.Remove(homeWork);
                    db.SaveChanges();
                    return BadRequest("Occured Error While Saving...Try Again");
                }
            }
            //save last changes => files path
            db.SaveChanges();
            return Ok("Home Work Edited Successfully");
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
            return Ok(db.HomeWorks.Include(q => q.Teacher).Include(q => q.Group).FirstOrDefault(f => f.Id == id));
        }
        [HttpGet]
        [Route("GetHomeWorkByTeacherId/{id}")]
        public IActionResult GetHomeWorkByTeacherId(int id)
        {
            return Ok(db.HomeWorks.
                Include(q => q.Group).
                Where(w => w.TeacherId == id).
                OrderByDescending(q => q.SendingDate).
                ToList());
        }

        [HttpGet]
        [Route("GetHomeWorkByStudentId/{id}")]
        public IActionResult GetHomeWorkByStudentId(int id)
        {
            //get all gorups that current student is in
            var ugs = db.UsersGroups.Where(q => q.UserId == id).ToList();
            var homeworks = new List<HomeWork>();
            ugs.ForEach(f =>
            {
                homeworks.AddRange(db.HomeWorks.Include(q => q.Group).Include(q => q.Teacher)
                    .Where(q => q.GroupId == f.GroupId));
            });
            homeworks = homeworks.OrderBy(q => q.SendingDate).Reverse().ToList();
            return Ok(homeworks);
        }

       

        [HttpDelete]
        [Route("DeleteHomeWork")]
        public IActionResult DeleteHomeWork([FromBody] HomeWork hwr)
        {
            var hw = db.HomeWorks.SingleOrDefault(f => f.Id == hwr.Id);
            if (hw == null)
                return NotFound("This Homework not found!!");

            string mainRoot = Path.Combine(hosting.WebRootPath, "HomeWork",
                   "Teachers", $"{hw.GroupId}", $"{hw.TeacherId}", $"{hw.Id}");
            try
            {
                Directory.Delete(mainRoot,true);
            }
            catch (Exception ex)
            {
                return BadRequest("Error Occured While Delete HomeWork Files");
            }
            db.HomeWorks.Remove(hw);
            db.SaveChanges();
            return Ok("HomeWork Deleted Successfully !!");

        }


        [HttpGet]
        [Route("DownloadHomeWorkFile")]
        public ActionResult DownloadDocument(int homeworkId,int GroupId, int TeacherId, string name)
        {
            string url = hosting.WebRootPath
                + "/HomeWork/Teachers/" +
                GroupId + "/" +
                TeacherId + "/" +
                homeworkId + "/" +
                name;

            if (!System.IO.File.Exists(url))
                return NotFound("Not Found File");

            byte[] fileBytes = System.IO.File.ReadAllBytes(url);
            return File(fileBytes, "application/force-download", name);
        }

    }

}
