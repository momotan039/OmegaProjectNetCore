using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.Entity;
using OmegaProject.services;
using System.IO;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly MyDbContext db;

        public NewsController(MyDbContext db)
        {
            this.db = db;
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {

            var data = db.News.OrderByDescending(f => f.Date).Select(f => new
            {
                id = f.Id,
                title = f.Title,
                describe = f.Describe.Substring(0,100)+"...",
                imageUrl = f.ImageUrl,
                date = f.Date
            }).ToList();
            return Ok(data);
        }

        [HttpGet]
        [Route("GetOne/{id}")]
        public IActionResult GetOne(int id)
        {
            var _new=db.News.FirstOrDefault(f => f.Id == id);
            return Ok(_new);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Post(IFormFile file)
        {
            var s = new New();
            s.Title = HttpContext.Request.Form["title"];
            s.Date = System.DateTime.Parse(HttpContext.Request.Form["date"]);
            s.Describe = HttpContext.Request.Form["describe"];
            try
            {
                if (file != null)
                {
                    InitNecessaryFolders();
                    string nameFile = CustomizePathFile(MyTools.ImagesRoot + "\\News\\", file.FileName);
                    SaveFileOnServerStorage(MyTools.ImagesRoot + "\\News\\" + nameFile, file);
                    s.ImageUrl = "Images\\News\\" + nameFile;
                }
                db.News.Add(s);
                db.SaveChanges();
            }
            catch 
            {
                return BadRequest("خطأ في الخادم");
            }
            return Ok("New Activty Added Successfully");
        }


        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var s = db.News.FirstOrDefault(f => f.Id == id);
            if (s == null)
                return NotFound("Not Found This Activity");

            try
            {
                if (s.ImageUrl != null || s.ImageUrl == "")
                    System.IO.File.Delete(Path.Combine(MyTools.Root, s.ImageUrl));

                db.News.Remove(s);
                db.SaveChanges();
            }
            catch
            {
                return BadRequest("خطأ في الخادم");
            }
            return Ok(" Activty Deleted Successfully");
        }

        [HttpPut]
        [Route("EditOne")]
        public IActionResult Povst(IFormFile file)
        {
            var s = new New();
            s.Id = int.Parse(HttpContext.Request.Form["id"]);
            var t = db.News.FirstOrDefault(t => t.Id == s.Id);

            if (t == null)
                return NotFound("This Member Not Found");
            t.Title = HttpContext.Request.Form["title"];
            t.Date = System.DateTime.Parse(HttpContext.Request.Form["date"]);
            t.Describe = HttpContext.Request.Form["describe"];

            //remove last image if exist
            if(System.IO.File.Exists(MyTools.Root+"\\"+ t.ImageUrl))
                System.IO.File.Delete(MyTools.Root + "\\" + t.ImageUrl);

            try
            {
                if (file != null)
                {
                    InitNecessaryFolders();
                    string nameFile = CustomizePathFile(MyTools.ImagesRoot + "\\News\\", file.FileName);
                    t.ImageUrl = "Images\\News\\" + nameFile;
                    SaveFileOnServerStorage(MyTools.ImagesRoot + "\\News\\" + nameFile, file);
                }

                db.SaveChanges();
            }
            catch 
            {
                return BadRequest("خطأ في الخادم");
            }
            return Ok("Activty Edited Successfully");
        }


        private void SaveFileOnServerStorage(string path, IFormFile file)
        {


            using (var fs = new FileStream(path, FileMode.Create))
            {
                if (file != null)
                {
                    file.CopyTo(fs);
                }
            }

        }

        private string CustomizePathFile(string mainRoot, string file)
        {

            if (!System.IO.File.Exists(Path.Combine(mainRoot, file)))
                return file;

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
                    return file;

                i++;
            }

        }

        private void InitNecessaryFolders()
        {
            //check if exist Images folder and create it
            if (!Directory.Exists(MyTools.ImagesRoot))
                Directory.CreateDirectory(MyTools.ImagesRoot);

            //check if exist News folder and create it
            if (!Directory.Exists(MyTools.ImagesRoot + "\\News"))
                Directory.CreateDirectory(MyTools.ImagesRoot + "\\News");

        }
    }
}
