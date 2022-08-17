using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IHostingEnvironment hosting;

        public FilesController(IHostingEnvironment hosting)
        {
            this.hosting = hosting;
        }

        [Route("FileExist/{url}")]
        public bool FileExist(string url)
        {
             return System.IO.File.Exists(url);
        }

        [Route("DownloadDocument")]
        public ActionResult DownloadDocument(int groupId,int teacherId,string file)
        {
            //https://localhost:44327/api/Files/DownloadDocument
            string url = hosting.WebRootPath+"/HomeWork/Files/"+groupId+"/"+teacherId+"/"+file;
            byte[] fileBytes = System.IO.File.ReadAllBytes(url);
            return File(fileBytes, "application/force-download", file);
        }
    }
}
