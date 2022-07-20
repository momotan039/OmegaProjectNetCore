using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : ControllerBase
    {

        MyDbContext db;
        private readonly IWebHostEnvironment hosting;
        public GroupsController(MyDbContext db, IWebHostEnvironment hosting)
        {
            this.db = db;
            this.hosting = hosting;
        }

        [HttpGet]
        [Route("GetGroupsByUserId/{id}")]
        public IActionResult GetGroupsByUserId(int id)
        {
            var groups = new List<Group>();
            //get all groups that contain this user
            db.UsersGroups.Where(ug => ug.UserId == id).ToList().ForEach(ug =>
            {
                //get group from ug id and insert it to groups list
                groups.Add(db.Groups.Include(g=>g.Course).First(g=>g.Id==ug.GroupId));
            });
            return Ok(groups);
        }

        [HttpGet]
        [Route("GetGroupsByCourseId/{id}")]
        public IActionResult GetGroupsByCourseId(int id)
        {
            var groups = new List<Group>();
            //get all groups that Teaching this Topic
            db.UsersGroups.Where(ug => ug.UserId == id).ToList().ForEach(ug =>
            {
                //get group from ug id and insert it to groups list
                groups.Add(db.Groups.Include(g => g.Course).First(g => g.Id == ug.GroupId));
            });
            return Ok(groups);
        }


        [HttpGet]
        [Route("GetGroups")]
        public IActionResult GetGroupes()
        {
            return Ok(db.Groups.Include(g=>g.Course).ToList());
        }


        [HttpPost]
        [Route("PostGroup")]
        public IActionResult PostGroup([FromBody] Group group)
        {
            var temp = db.Groups.FirstOrDefault(x => x.Name == group.Name);
            if (temp != null)
                return BadRequest("Faild Added ...This Group Exist !!");
            //group.OpeningDate= System.DateTime.Now;
            db.Groups.Add(group);
            db.SaveChanges();
            return StatusCode(200);
        }

        [HttpDelete]
        [Route("DeleteGroup/{id}")]
        public IActionResult DeleteGroup(int id)
        {

            //check if user Existed
            var temp = db.Groups.FirstOrDefault(x => x.Id == id);
            if (temp == null)
                return BadRequest("Faild Deleted ...This Group not Exist !!");


            //delete HomeWork Files that Releated to this Group
            var path = hosting.WebRootPath + "\\HomeWork" + "\\Files"+"\\"+id;
            try
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
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
            return StatusCode(200);
        }

        [HttpPut]
        [Route("EditGroup")]
        public IActionResult EditGroup([FromBody] Group group)
        {
            //check if user Existed
            var temp = db.Groups.FirstOrDefault(x => x.Id == group.Id);
            if (temp == null)
                return BadRequest("Faild Editing ...This Group not Exist !!");
            temp.Name = group.Name;
            temp.ClosingDate = group.ClosingDate;
            temp.OpeningDate = group.OpeningDate;
            temp.CourseId = group.CourseId;
            db.SaveChanges();
            return StatusCode(200);
        }
    }
}
