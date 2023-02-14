using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.Entity;
using OmegaProject.services;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactUsMessagesController : ControllerBase
    {
        private readonly MyDbContext db;

        public ContactUsMessagesController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("GetAll")]
        public IActionResult GetAll()
        {

           var data=db.ContactUsMessages.OrderByDescending(f=>f.SendingDate).ToList();
            return Ok(data);
        }

        [HttpPost]
        [Route("Add")]
        public IActionResult Post([FromBody] ContactUsMessage cum)
        {
            cum.SendingDate = System.DateTime.Now;
            try
            {
                db.ContactUsMessages.Add(cum);
                db.SaveChanges();
            }
            catch
            {
                return BadRequest("خطأ في الخادم");
            }
            return Ok("تم ارسالة الرسالة بنجاح انتظر ردنا باقرب وقت");
        }

        [HttpDelete]
        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            
            var cum=db.ContactUsMessages.FirstOrDefault(f=>f.Id==id);
            if (cum == null)
                return NotFound("Not Found Item");
            try
            {
                db.ContactUsMessages.Remove(cum);
                db.SaveChanges();
            }
            catch
            {
                return BadRequest("Occured Error While Removing Message");
            }
            return Ok("Inquirie Deleted Successfully");
        }
    }
}
