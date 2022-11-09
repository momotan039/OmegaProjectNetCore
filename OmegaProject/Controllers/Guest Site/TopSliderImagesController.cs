using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.Entity.Guest_Site;
using OmegaProject.services;
using System.IO;
using System.Linq;

namespace OmegaProject.Controllers.Guest_Site
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopSliderImagesController : ControllerBase
    {
        private readonly MyDbContext db;

        public TopSliderImagesController(MyDbContext db)
        {
            this.db = db;
        }
        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {

            var data = db.TopsliderImages.OrderBy(f=>f.Order).ToList();
            return Ok(data);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Post(IFormFile file)
        {

        var s = new TopSliderImage();
            s.Title = HttpContext.Request.Form["title"];
            s.Order =int.Parse(HttpContext.Request.Form["order"]);
            try
            {
                if (file != null)
                {
                    InitNecessaryFolders();
                    string nameFile = CustomizePathFile(MyTools.ImagesRoot + "\\TopSlider\\", file.FileName);
                    SaveFileOnServerStorage(MyTools.ImagesRoot + "\\TopSlider\\" + nameFile, file);
                    s.ThumbImage = "Images\\TopSlider\\" + nameFile;
                }
                db.TopsliderImages.Add(s);
                db.SaveChanges();
            }
            catch
            {
                return BadRequest("Erorr While Adding Image");
            }
            return Ok("Image Slider Added Successfully");
        }


        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var s = db.TopsliderImages.FirstOrDefault(f => f.Id == id);
            if (s == null)
                return NotFound("Not Found This Image Slider");

            try
            {
                if (s.ThumbImage != null || s.ThumbImage == "")
                    System.IO.File.Delete(Path.Combine(MyTools.Root, s.ThumbImage));

                db.TopsliderImages.Remove(s);
                db.SaveChanges();
            }
            catch
            {
                return BadRequest("Erorr While Adding Image");
            }
            return Ok("Image Slider Deleted Successfully");
        }

        [HttpPut]
        [Route("EditOne")]
        public IActionResult Povst(IFormFile file)
        {
            var s = new TopSliderImage();
            s.Id = int.Parse(HttpContext.Request.Form["id"]);
            var t = db.TopsliderImages.FirstOrDefault(t => t.Id == s.Id);

            if (t == null)
                return NotFound("This Image Not Found");

            t.Title = HttpContext.Request.Form["title"];
            t.Order = int.Parse(HttpContext.Request.Form["order"]);
            

            try
            {
                if (file != null)
                {
                    //remove last image if exist
                    if (System.IO.File.Exists(MyTools.Root + "\\" + t.ThumbImage))
                        System.IO.File.Delete(MyTools.Root + "\\" + t.ThumbImage);

                    InitNecessaryFolders();
                    string nameFile = CustomizePathFile(MyTools.ImagesRoot + "\\TopSlider\\", file.FileName);
                    t.ThumbImage = "Images\\TopSlider\\" + nameFile;
                    SaveFileOnServerStorage(MyTools.ImagesRoot + "\\TopSlider\\" + nameFile, file);
                }

                db.SaveChanges();
            }
            catch
            {
                return BadRequest("Erorr While Adding Image");
            }
            return Ok("Image Slider Edited Successfully");
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

            //check if exist TopSlider Images folder and create it
            if (!Directory.Exists(MyTools.ImagesRoot + "\\TopSlider"))
                Directory.CreateDirectory(MyTools.ImagesRoot + "\\TopSlider");

        }
    }
}
