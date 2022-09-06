using Cake.Core.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using OmegaProject.Entity;
using OmegaProject.services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Path = System.IO.Path;

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
        [Route("SubmitFiles")]
        public async Task<IActionResult> SubmitFiles(IFormFile[] files)
        {
            string path = Path.Combine(hosting.WebRootPath, "HomeWork",
                  "Submited");

            var hws = new HomeWorkStudent();
            hws.HomeWorkId = int.Parse(HttpContext.Request.Form["homeWorkId"]);
            hws.StudentId = int.Parse(HttpContext.Request.Form["studentId"]);

            int GroupId = int.Parse(HttpContext.Request.Form["groupId"]);

            db.HomeWorkStudents.Add(hws);
            db.SaveChanges();

            //get paths of uploaded files
            if (files.Length != 0)
            {
                InitNecessaryFolders(path, hws, GroupId);

                string mainRoot = Path.Combine(hosting.WebRootPath, "HomeWork",
                    "Submited", $"{GroupId}", $"{hws.StudentId}", $"{hws.HomeWorkId}");

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
                    db.HomeWorkStudents.Remove(hws);
                    db.SaveChanges();
                    return BadRequest("Occured Error While Saving...Try Again");
                }
            }

            db.SaveChanges();
            return Ok("Files Submited Successfully");
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

        private void InitNecessaryFolders(string path, HomeWorkStudent hws, int groupId)
        {
            //check if exist group id folder and create it
            if (!Directory.Exists(path + $"\\{groupId}"))
                Directory.CreateDirectory(path + $"\\{groupId}");

            //check if exist student id and create it
            if (!Directory.Exists(path + $"\\{groupId}" + $"\\{hws.StudentId}"))
                Directory.CreateDirectory(path + $"\\{groupId}" + $"\\{hws.StudentId}");

            //check if exist submited homework id and create it
            if (!Directory.Exists(path + $"\\{groupId}" + $"\\{hws.StudentId}" + $"\\{hws.HomeWorkId}"))
                Directory.CreateDirectory(path + $"\\{groupId}" + $"\\{hws.StudentId}" + $"\\{hws.HomeWorkId}");
        }


        [HttpDelete]
        [Route("DeleteSubmited")]
        public ActionResult DeleteSubmited([FromBody] dynamic homeworkStudent)
        {
            int id = homeworkStudent.id;

            var hws = db.HomeWorkStudents.SingleOrDefault(f => f.Id == id);
            if (hws == null)
                return NotFound("Not Found Submited HomeWork");

            string mainRoot = Path.Combine(hosting.WebRootPath, "HomeWork", 
                    "Submited", $"{homeworkStudent.groupId}", $"{hws.StudentId}", $"{hws.HomeWorkId}");

            //remove all related files
            
            if(Directory.Exists(mainRoot))
            Directory.Delete(mainRoot, true);
            //try
            //{

            //}
            //catch (Exception ex)
            //{
            //    return BadRequest("Occured Error While Deletion");
            //}

            db.HomeWorkStudents.Remove(hws);
            db.SaveChanges();
            return Ok("Submited Homewrok Deteted Successfully");
        }

        [HttpGet]
        [Route("GetSubmitedStudentsById/{id}")]
        public ActionResult GetSubmitedStudentById(int id)
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

                    //get  student properties
                    var homeworkStudent = Submited_Students.FirstOrDefault(f => f.StudentId == q.UserId);
                    if (homeworkStudent != null)
                    {
                        s.submited = "Yes";
                        s.pathFiles = homeworkStudent.FilesPath;
                        s.homeWorkStudentId = homeworkStudent.Id;
                    }
                    else
                        s.submited = "No";
                    students.Add(s);
                });

            return Ok(students.OrderByDescending(d => ((dynamic)d).submited));
        }

        [HttpGet]
        [Route("GetSubmitStudentByself")]
        public ActionResult GetSubmitStudentByself(int studentId, int homeWorkId)
        {
            var data = db.HomeWorkStudents
                .Include(q=>q.Student)
                .FirstOrDefault(q=>q.StudentId== studentId && q.HomeWorkId== homeWorkId);

            if (data == null)
                return Ok(null);

            dynamic s = new ExpandoObject();
            s.firstName = data.Student.FirstName;
            s.lastName = data.Student.LastName;
            s.idCard = data.Student.IdCard;
            s.email = data.Student.Email;

            if ( data.FilesPath != "")
            {
                s.submited = "Yes";
                s.pathFiles = data.FilesPath;
                s.homeWorkStudentId = data.Id;
            }
            else
                s.submited = "No";
            return Ok(s);
        }
        private string GetContentType(string path)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if (!provider.TryGetContentType(path, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
        [HttpGet, DisableRequestSizeLimit]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadDocument(int homeworkId, int GroupId, int StudentId, string name)
        {
            string url = hosting.WebRootPath
                + "/HomeWork/Submited/" +
                GroupId + "/" +
                StudentId + "/" +
                homeworkId + "/" +
                name;

            //using (WebClient wc = new WebClient())
            //{
            //    wc.Headers.Add("User-Agent: Test");
            //     wc.DownloadFile(url,name);
            //}
            var memory = new MemoryStream();
            using (var stream = new FileStream(url, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return  Ok(File(memory, GetContentType(url), name)) ;
            //using HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            //using Stream streamToReadFrom = await response.Content.ReadAsStreamAsync();
            //using Stream streamToWriteTo = System.IO.File.Open(url, FileMode.Create);
            //await streamToReadFrom.CopyToAsync(streamToWriteTo);

            //Stream stream =System.IO.File.OpenRead(url);

            //if (stream == null)
            //    return NotFound("Not Found File");

            //return new FileStreamResult(stream, "application/octet-stream");

            //if (!System.IO.File.Exists(url))
            //    return NotFound("Not Found File");

            //byte[] fileBytes = System.IO.File.ReadAllBytes(url);
            //return File(fileBytes, "application/force-download", name);
        }


        [HttpGet]
        [Route("DownloadFileByPath")]
        public ActionResult DownloadFileByPath([FromQuery]string url)
        {
            if (!System.IO.File.Exists(url))
                return NotFound("Not Found File");

            byte[] fileBytes = System.IO.File.ReadAllBytes(url);
            return File(fileBytes, "application/force-download", Path.GetFileName(url));
        }
    }
}
