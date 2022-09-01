using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.Entity;
using OmegaProject.services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeWorkStudentsController : ControllerBase
    {
        private readonly MyDbContext db;
        private readonly IHostingEnvironment hosting;

        public HomeWorkStudentsController(MyDbContext db, IHostingEnvironment hosting)
        {
            this.db = db;
            this.hosting = hosting;
        }
        [HttpGet]
        [Route("SubmitedHomeworksStudent/{id?}")]
        public ActionResult SubmitedHomeworksStudent(int id = -1)
        {
            if (id == -1)
                return Ok(db.HomeWorkStudents.Include(q => q.Student));
            //Get Submited homeworks (homework id)
            var data = db.HomeWorkStudents.
                Include(q => q.Student).
                Where(f => f.HomeWorkId == id).ToList();

            if (data.Count == 0)
                return NotFound("Not found Relation ship between student and homework");
            return Ok(data);
        }

        [HttpPost]
        [Route("SubmitHomeworkStudent")]
        public ActionResult SubmitHomeworkStudent(IFormFile[] files)
        {

            string path = Path.Combine(hosting.WebRootPath, "HomeWork",
                  "Submited");

            var hws = new HomeWorkStudent();
            hws.HomeWorkId = int.Parse(HttpContext.Request.Form["homeWorkId"]);
            hws.StudentId = int.Parse(HttpContext.Request.Form["studentId"]);

            //get paths of uploaded files
            if (files.Length != 0)
            {
                InitNecessaryFolders(path, hws);

                string mainRoot = Path.Combine(hosting.WebRootPath, "HomeWork",
                    "Submited", $"{hws.StudentId}", $"{hws.HomeWorkId}");

                foreach (var file in files)
                {
                    path = CustomizeNameFile(mainRoot, file.FileName);
                    hws.FilesPath += path + "\n";
                }
                // if user Uploaded files .. save it
                var paths = hws.FilesPath.Split('\n').ToArray();
                try
                {
                    SaveFileOnServerStorage(paths, files);
                }
                catch (Exception r)
                {
                    return BadRequest("Occured Error While Saving...Try Again");
                }
            }

            db.HomeWorkStudents.Add(hws);
            db.SaveChanges();
            return Ok("Homewrok Submited Successfully");
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

        private void InitNecessaryFolders(string path, HomeWorkStudent hws)
        {
            //check if exist Student id folder adn create it
            if (!Directory.Exists(path + $"\\{hws.StudentId}"))
                Directory.CreateDirectory(path + $"\\{hws.StudentId}");

            //check if exist submited homework id and create it
            if (!Directory.Exists(path + $"\\{hws.StudentId}" + $"\\{hws.HomeWorkId}"))
                Directory.CreateDirectory(path + $"\\{hws.StudentId}" + $"\\{hws.HomeWorkId}");
        }


        [HttpDelete]
        [Route("DeleteSubmitedHomeworkStudent/{id}")]
        public ActionResult SubmitHomeworkStudent(int id)
        {
            var hws = db.HomeWorkStudents.SingleOrDefault(f => f.Id == id);
            if (hws == null)
                return NotFound("Not Found Submited HomeWork");

            string mainRoot = Path.Combine(hosting.WebRootPath, "HomeWork",
                    "Submited", $"{hws.StudentId}", $"{hws.HomeWorkId}");

            try
            {
                //remove all related files
                Directory.Delete(mainRoot, true);
            }
            catch (Exception ex)
            {
                return BadRequest("Occured Error While Deletion");
            }

            db.HomeWorkStudents.Remove(hws);
            db.SaveChanges();
            return Ok("Submited Homewrok Deteted Successfully");
        }

        [HttpGet]
        [Route("GetSubmitedHomeworkById/{id}")]
        public ActionResult GetSubmitedHomeworkById(int id)
        {
            var homework = db.HomeWorks.SingleOrDefault(f => f.Id == id);
            var Submited_Students = db.HomeWorkStudents
                .Include(q => q.Student)
                .Where(q => q.HomeWorkId == id).ToList();

            var students = new List<ExpandoObject>();
            //get students
            db.UsersGroups
                .Include(q => q.User)
                .Where(q => q.GroupId == homework.GroupId
                    && q.User.RoleId == 3)
                .ToList().ForEach(q =>
                {
                    dynamic s = new ExpandoObject();
                    s.firstName = q.User.FirstName;
                    s.lastName = q.User.LastName;
                    s.idCard = q.User.IdCard;
                    s.email = q.User.Email;
                    if (Submited_Students.Exists(f => f.StudentId == q.UserId))
                        s.submited = "Yes";
                    else
                        s.submited = "No";
                    students.Add(s);
                });

            return Ok(students.OrderByDescending(d => ((dynamic)d).submited));
        }
    }
}
