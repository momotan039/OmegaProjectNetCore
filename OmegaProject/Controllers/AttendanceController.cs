

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.Entity;
using OmegaProject.services;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly MyDbContext db;

        public AttendanceController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [Route("GetAll/{groupId}/{date?}")]
        public IActionResult GetAll(int groupId,string date)
        {
            DateTime _date = DateTime.Now.Date;
            try
            {
                _date = date == null ? _date : DateTime.Parse(date);
            }
            catch
            {
                return Ok();
            }

            string name=CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(8);

            var data = db.UsersGroups.Where(f => f.GroupId == groupId && f.User.RoleId==3)
                .Select(x => new
                {
                    idCardStudent=x.User.IdCard,
                    studentId=x.User.Id,
                    studentName = x.User.FirstName + " " + x.User.LastName,
                    status = db.Attendances.
                                FirstOrDefault(f=>f.Date==_date && groupId==f.GroupId && x.UserId==f.StudentId)
                                .Status,
                    //date =_date==null?DateTime.Now.Date:_date,
                    date=_date,
                    groupId = x.GroupId,
                    note= db.Attendances.
                                FirstOrDefault(f => f.Date == _date && groupId == f.GroupId && x.UserId == f.StudentId)
                                .Note
                }).ToList();

            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllUser/{groupId}/{studentId}/{date?}")]
        public IActionResult GetAll(int groupId,int studentId, string date)
        {
            DateTime _date= DateTime.Now.Date;
            try
            {
                 _date = date == null ? _date : DateTime.Parse(date);
            }
            catch
            {
                return Ok();
            }

            var data = db.Attendances.Include(f=>f.Group).Where(f => f.StudentId == studentId &&f.GroupId==groupId && f.Date.Month==_date.Month)
                .Select(x => new
                {
                    //idCardStudent = x.User.IdCard,
                    //studentId = x.User.Id,
                    //studentName = x.User.FirstName + " " + x.User.LastName,
                    status =x.Status == true ? "Present" : "Absent",
                    //date =_date==null?DateTime.Now.Date:_date,
                    date = x.Date,
                    groupId = x.GroupId,
                    group =x.Group.Name,
                    note = x.Note
                }).OrderByDescending(f=>f.date).ToList();

            return Ok(data);
        }

        [Obsolete]
        [HttpGet]
        [Route("GetAttendanceStatistics/{groupId}/{studentId}")]
        public IActionResult GetAttendanceStatistics(int groupId, int studentId)
        {
            var d=db.Attendances.FromSql($"select count(*),Month(Attendances.Date) from Attendances group by Month(Attendances.Date)").ToList();
           
            var data = db.Attendances.Include(f => f.Group)
                .Where(f => f.StudentId == studentId && f.GroupId == groupId).GroupBy(f=>f.Date.Month)
                .Select(x => new
                {
                    x.Key,
                    count=x.Count()
                })
                .ToList();

            return Ok(data);
        }

        [HttpPost]
        [Route("Post")]
        public IActionResult Post([FromBody]Attendance[] att)
        {
            att.ToList().ForEach(x =>
            {
                var found = db.Attendances.FirstOrDefault(f=>x.Date==f.Date && x.GroupId==f.GroupId && x.StudentId==f.StudentId);
                if(found!=null)
                {
                    found.Note = x.Note;
                    found.Status=x.Status;
                }
                else
                db.Attendances.Add(x);
            });
            db.SaveChanges();
            return Ok("Attendance Save Successfully");
        }

        [HttpPut]
        [Route("Edit/{id}/{status}")]
        public IActionResult Edit(int id,bool status)
        {
            var att = db.Attendances.FirstOrDefault(f => f.StudentId == id);
            att.Status = status;
            db.SaveChanges();

            return Ok("Attendance Edited Successfully");
        }


        [HttpGet]
        [Route("GetGroups")]
        public async Task<IActionResult> GetGroupes()
        {
            var result = db.Groups
                .OrderByDescending(g => g.OpeningDate)
                .Select(f => new
            {
                id=f.Id,
                name= f.Name
            }).ToList();

            return Ok(result);
        }


    }
}
