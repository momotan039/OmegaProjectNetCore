using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.Entity;
using OmegaProject.services;
using System;
using System.IO;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly MyDbContext db;

        public StaffsController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {

            var data = db.Staffs.ToList();
            return Ok(data);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Post(IFormFile file)
        {
            var s = new Staff();
            s.Name=HttpContext.Request.Form["name"];
            s.Work=HttpContext.Request.Form["Work"];
            s.About=HttpContext.Request.Form["About"];
            try
            {
                if(file!=null)
                {
                    InitNecessaryFolders();
                    string nameFile = CustomizePathFile(MyTools.ImagesRoot + "\\Staff\\", file.FileName);
                    SaveFileOnServerStorage(MyTools.ImagesRoot + "\\Staff\\" + nameFile, file);
                    s.ImageUrl = "Images\\Staff\\" + nameFile;
                }
                db.Staffs.Add(s);
                db.SaveChanges();
            }
            catch(Exception exception)
            {
                return BadRequest("خطأ في الخادم");
            }
            return Ok("New Member Added To Staff Successfully");
        }


        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var s = db.Staffs.FirstOrDefault(f => f.Id == id);
            if (s == null)
                return NotFound("Not Found This Member");

            try
            {
                if(s.ImageUrl!=null ||s.ImageUrl=="")
                System.IO.File.Delete(Path.Combine(MyTools.Root,s.ImageUrl));

                db.Staffs.Remove(s);
                db.SaveChanges();
            }
            catch
            {
                return BadRequest("خطأ في الخادم");
            }
            return Ok("تم حذف هذا العضو بنجاح");
            return Ok("This Member Deleted From Staff Successfully");
        }

        [HttpPut]
        [Route("EditOne")]
        public IActionResult Povst(IFormFile file)
        {
            var s = new Staff();
            s.Id = int.Parse(HttpContext.Request.Form["id"]);
            var t=db.Staffs.FirstOrDefault(t=>t.Id == s.Id);

            if (t == null)
                return NotFound("This Member Not Found");

            t.Name = HttpContext.Request.Form["name"];
            t.Work = HttpContext.Request.Form["Work"];
            t.About = HttpContext.Request.Form["About"];

            //remove last image if exist
            if (System.IO.File.Exists(MyTools.Root + "\\" + t.ImageUrl))
                System.IO.File.Delete(MyTools.Root + "\\" + t.ImageUrl);

            try
            {
                if(file!=null)
                {
                    InitNecessaryFolders();
                    string nameFile = CustomizePathFile(MyTools.ImagesRoot + "\\Staff\\", file.FileName);
                    t.ImageUrl = "Images\\Staff\\" + nameFile;
                    SaveFileOnServerStorage(MyTools.ImagesRoot + "\\Staff\\" + nameFile, file);
                }
               
                db.SaveChanges();
            }
            catch (Exception exception)
            {
                return BadRequest("خطأ في الخادم");
            }
            return Ok("Member Edited Successfully");
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

            //check if exist staff folder and create it
            if (!Directory.Exists(MyTools.ImagesRoot+"\\Staff"))
                Directory.CreateDirectory(MyTools.ImagesRoot + "\\Staff");

        }


    }
}
