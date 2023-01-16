using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmegaProject.DTO;
using OmegaProject.services;
using System;
using System.IO;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly MyDbContext db;
        private readonly JwtService jwt;

        public AccountController(MyDbContext db,JwtService jwt)
        {
            this.db = db;
            this.jwt = jwt;
        }
        [HttpPost]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(UserLogInDTO model)
        {
            model.Email = jwt.GetTokenClaims();
            var user=db.Users.FirstOrDefault(q=>q.Email==model.Email);
            if(user==null)
                return NotFound("User Not Exist!!");
            user.Password = MyTools.CreateHashedPassword(model.Password);
            db.SaveChanges();
            return Ok("Password Changed Successfully!");
        }

        [HttpPost]
        [Route("ForgetPassword")]
        public IActionResult ForgetPassword([FromBody]UserLogInDTO u)
        {

            var user = db.Users.FirstOrDefault(q => q.Email == u.Email);

            if (user == null)
                return NotFound("User Not Exist!!");

            string token=jwt.GenerateToken(u.Email, false,new System.TimeSpan(0,1,0));

            token = token.Replace("Bearer ", "");

            bool success = MyTools.SendResetPassMail(token, u.Email);

            if(!success)
            return BadRequest("Occured Error While Sending Link to Mail!!");

            return Ok("Link Sended to Mail Successfully!!\nPlease Cheak Your Mail");
        }


        [HttpPut]
        [Route("EditImageProfile")]
        public IActionResult EditImageProfile(IFormFile image)
        {
            if (image == null)
                return BadRequest("Image is null");

            User user = db.Users.FirstOrDefault(g => g.Id == int.Parse(jwt.GetTokenClaims()));

            if (user == null)
                return BadRequest("Not Found User");

            //check Exist Folder Images
            if (!Directory.Exists(MyTools.ImagesRoot))
                Directory.CreateDirectory(MyTools.ImagesRoot);

            string Image_path = Path.Combine(MyTools.ImagesRoot, "Users");

            //check Exist Folder Images for Users
            if (!Directory.Exists(Image_path))
                Directory.CreateDirectory(Image_path);

            Random r = new Random();

            //set image name by user id
            //string imageName = user.Id+Path.GetFileNameWithoutExtension(image.FileName) + Path.GetExtension(image.FileName);
            string imageName = r.Next(10000000, 1000000000) + Path.GetExtension(image.FileName);
           
            Image_path = Path.Combine(Image_path, imageName);

            using (var fs = new FileStream(Image_path, FileMode.Create))
            {
                image.CopyTo(fs);
            }

            //save path image to Database
            user.ImageProfile = "/Images/Users/" + imageName;
            db.SaveChanges();
            return Ok("Your Image Changed Successfully");
        }


        [HttpPut]
        [Route("EditImageProfileGroup")]
        public IActionResult 
            
            Group(IFormFile image)
        {
            if (image == null)
                return BadRequest("Image is null");

            int id_group = int.Parse(HttpContext.Request.Form["idGroup"]);
            Group group = db.Groups.FirstOrDefault(g => g.Id == id_group);

            if(group==null)
                return BadRequest("Not Found Group");

            //check Exist Folder Images
            if (!Directory.Exists(MyTools.ImagesRoot))
                Directory.CreateDirectory(MyTools.ImagesRoot);
            
            string Image_path = Path.Combine(MyTools.ImagesRoot, "Groups");

            //check Exist Folder Images for groups
            if (!Directory.Exists(Image_path))
                Directory.CreateDirectory(Image_path);

            Random r = new Random();
            //set image name by group id
            string imageName = r.Next(10000000, 1000000000) + Path.GetExtension(image.FileName);

            Image_path = Path.Combine(Image_path,imageName);

            using (var fs = new FileStream(Image_path,FileMode.Create))
            {
                    image.CopyTo(fs);
            }

            //save path image to Database
            group.ImageProfile = "/Images/Groups/" + imageName;
            db.SaveChanges();
            return Ok("Group Image Changed Successfully");
        }

        private string CustomizeImageFile(string ImageName,string ImageProfile)
        {
            string ext = Path.GetExtension(ImageName);
            int i = 1;
            while (true)
            {
                ImageName = Path.GetFileNameWithoutExtension(ImageName);//f.text
                if (i == 1)
                    ImageName += "_1";
                else
                    ImageName = ImageName.Replace($"_{i - 1}", $"_{i}");
                ImageName += ext;
                i++;
            }
        }
    }
}
