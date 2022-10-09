using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GroupsController : ControllerBase
    {

        MyDbContext db;
        private readonly IWebHostEnvironment hosting;
        private readonly JwtService jwt;

        public GroupsController(MyDbContext db, IWebHostEnvironment hosting, JwtService jwt)
        {
            this.db = db;
            this.hosting = hosting;
            this.jwt = jwt;
        }

        [HttpGet]
        [Route("GetGroupsByUserId/{id?}")]
        public async Task<IActionResult> GetGroupsByUserIdAsync(int id = -1)
        {
            if (id == -1)
                id = int.Parse(jwt.GetTokenClaims());

            var user = db.Users.SingleOrDefault(r => r.Id == id);

            if (user == null)
                return NotFound("User Not Exist");

            var groups = new List<Group>();
            if (user.RoleId == 1)
            {
                var gs = db.Groups.Include(g => g.Course).ToList();
                return Ok(gs);

            }

            //get all groups that contain this user


            //db.UsersGroups.Where(ug => ug.UserId == id).ToList().ForEach(ug =>
            //{
            //    //get group from ug id and insert it to groups list
            //    groups.Add(db.Groups.Include(g => g.Course).First(g => g.Id == ug.GroupId));
            //});
            var groups2 = db.Groups
                .Include(q => q.Course)
                .Include(q => q.UserGroups)
                .ThenInclude(q => q.User)
                .Where(q => q.UserGroups.Any(f => f.UserId!=id &&  f.User.RoleId != 1)).ToList()
                ;

            return Ok(groups2);
        }

        [HttpGet]
        [Route("GetGroupsByCourseId/{id}")]
        public IActionResult GetGroupsByCourseId(int id)
        {
            var groups = db.Groups.Include(q=>q.UserGroups).Where(q => q.CourseId == id).ToList();
            //get all groups that Teaching this Topic

            return Ok(groups);
        }



        [HttpGet]
        [Route("GetGroups")]
        public IActionResult GetGroupes()
        {
            return Ok(db.Groups.Include(g => g.Course).
                Include(g => g.UserGroups).
                ThenInclude(d => d.User).ToList());
        }

        [HttpGet]
        [Route("GetGroupById/{id}")]
        public IActionResult GetGroupById(int id)
        {
            var g = db.Groups.Include(q => q.UserGroups).
                ThenInclude(q => q.User).
                ThenInclude(q => q.Role).
                SingleOrDefault(x => x.Id == id);
            if (g == null)
                return NotFound("Not Found Group");
            return Ok(g);
        }



        [HttpPost]
        [Route("PostGroup")]
        public IActionResult PostGroup([FromBody] Group group)
        {
            //var temp = db.Groups.FirstOrDefault(x => x.Name == group.Name);
            //if (temp != null)
            //    return BadRequest("Faild Added ...This Group Exist !!");
            //group.OpeningDate= System.DateTime.Now;
            if (group == null)
                return BadRequest("Fiald Adding Group");
            db.Groups.Add(group);
            db.SaveChanges();
            return Ok("Group Added Successfully");
        }

        [HttpDelete]
        [Route("DeleteGroup/{id}")]
        public IActionResult DeleteGroup(int id)
        {

            //check if user Existed
            var temp = db.Groups.FirstOrDefault(x => x.Id == id);
            if (temp == null)
                return NotFound("Faild Deleted ...This Group not Exist !!");


            //delete HomeWork Files that Releated to this Group
            var path = hosting.WebRootPath + "\\HomeWork" + "\\Files" + "\\" + id;
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path,true);
                }
            }
            catch (Exception r)
            {
                return BadRequest(r.Message);
            }

            //foreach (var file in Directory.GetFiles(imagesFolder))
            //{
            //    System.IO.File.Delete(file);
            //}
            //delete this group
            db.Groups.Remove(temp);
            db.SaveChanges();
            return Ok("Group Deleted Successfully");
        }

        [HttpPut]
        [Route("EditGroup")]
        public IActionResult EditGroup([FromBody] Group group)
        {
            //check if user Existed
            var temp = db.Groups.FirstOrDefault(x => x.Id == group.Id);
            if (temp == null)
                return NotFound("Faild Editing ...This Group not Exist !!");
            temp.Name = group.Name;
            temp.ClosingDate = group.ClosingDate;
            temp.OpeningDate = group.OpeningDate;
            temp.CourseId = group.CourseId;
            db.SaveChanges();
            return Ok("Group Edited Successfully");
        }
    }
}
