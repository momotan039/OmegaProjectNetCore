

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmegaProject.DTO;
using OmegaProject.Entity;
using OmegaProject.services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OmegaProject.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        private readonly MyDbContext db;
        private readonly JwtService jwt;

        public AttendanceController(MyDbContext db,JwtService jwt, IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this.jwt = jwt;
            this.httpContextAccessor = httpContextAccessor;
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

            if(studentId==-1)
            {

                var _data = db.Attendances.Include(f => f.Group).
                    Include(f => f.Student).
                    Where(f => f.GroupId == groupId && f.Date.Month == _date.Month)
           .Select(x => new
           {
               idCardStudent = x.Student.IdCard,
               studentName = x.Student.FirstName + " " + x.Student.LastName,
               status = x.Status == true ? "Present" : "Absent",
                    date = x.Date,
               group = x.Group.Name,
               note = x.Note
           }).OrderByDescending(f => f.date).ToList();

                return Ok(_data);

            }

            var data = db.Attendances.Include(f => f.Group).Where(f => f.StudentId == studentId && f.GroupId == groupId && f.Date.Month == _date.Month)
           .Select(x => new
           {
                    //idCardStudent = x.User.IdCard,
                    //studentId = x.User.Id,
                    //studentName = x.User.FirstName + " " + x.User.LastName,
                    status = x.Status == true ? "Present" : "Absent",
                    //date =_date==null?DateTime.Now.Date:_date,
                    date = x.Date,
               groupId = x.GroupId,
               group = x.Group.Name,
               note = x.Note
           }).OrderByDescending(f => f.date).ToList();

            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllUsersPerMonth/{groupId}/{date?}")]
        public async Task<IActionResult> GetAllUsers(int groupId, string date)
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

            var userGroups =await db.UsersGroups.Where(g => g.GroupId == groupId).ToListAsync();

            string monthStr = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(_date.Month);

            var data = db.Attendances.Include(f => f.Group)
                .Include(f => f.Student)
                .Where(f => f.GroupId == groupId &&
                f.Date.Month == _date.Month
                ).ToList();

            var filteredData = new List<Attendance>();
            data.ForEach(x =>
            {
                if (userGroups.Any(q => q.UserId == x.StudentId))
                    filteredData.Add(x);
            });

            var _data=filteredData.GroupBy(f => new
            {
                f.StudentId,
                f.Student.FirstName,
                f.Student.LastName,
                f.Date.Month,
            }).Select(x => new
            {
                studentId=x.Key.StudentId,
                monthInt=x.Key.Month,
                count=x.Count(),
                studentName=x.Key.FirstName+" "+x.Key.LastName
            }).ToList();

            var studentsName = new List<string>();
            var attendances = new List<float>();
            _data.ForEach(d =>
            {
                float percentPresent = db.Attendances
                .Where(f => f.Date.Month == _date.Month && groupId == f.GroupId && f.StudentId == d.studentId && f.Status).Count();
                float totalPresentByMonth = percentPresent / d.count;
                studentsName.Add(d.studentName);
                attendances.Add(totalPresentByMonth);
            });

            var result = new
            {
                label = monthStr,
                labels = studentsName,
                data = attendances
            };

            return Ok(result);
        }

        //[HttpGet]
        //[Route("GetAttendanceStatistics/{groupId}/{studentId}")]
        public async Task<object> GetAttendanceStatistics(int groupId, int studentId)
        {
            var data = await db.Attendances
                .Where(f => f.StudentId == studentId && f.GroupId == groupId).GroupBy(f=>f.Date.Month)
                .Select(x => new
                {
                    month=CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Key),
                    month_int=x.Key,
                    count=x.Count()
                })
                .ToListAsync();

            var months = new List<string>();
            var counts = new List<float>();
            data.ForEach(x =>
            {
                months.Add(x.month);
                int countPresents = db.Attendances.
                Where(f => f.StudentId == studentId && f.GroupId == groupId && f.Date.Month == x.month_int && f.Status == true).
                Count();
                 float _counts = (float)countPresents / x.count;
                counts.Add(_counts);
            });

            var _result = new
            {
                months = months,
                counts = counts
            };

            return _result;
        }

        public async Task<object> GetAttendanceStatistics2(int groupId, int studentId)
        {
            var allMonths = Enumerable.Range(1, 12)
                .Select(monthNumber => new
                {
                    month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(monthNumber),
                    month_int = monthNumber
                })
                .ToList();

            var attendanceCounts = await db.Attendances
                .Where(f => f.StudentId == studentId && f.GroupId == groupId)
                .GroupBy(f => f.Date.Month)
                .Select(x => new
                {
                    month_int = x.Key,
                    count = x.Count()
                })
                .ToListAsync();

            var counts = new List<string>();
            allMonths.ForEach(month =>
            {
                var attendanceCount = attendanceCounts.FirstOrDefault(x => x.month_int == month.month_int);
                int countPresents = attendanceCount != null
                    ? db.Attendances
                        .Where(f => f.StudentId == studentId && f.GroupId == groupId && f.Date.Month == month.month_int && f.Status == true)
                        .Count()
                    : 0;

                string count = attendanceCount != null && attendanceCount.count > 0
            ? ((float)countPresents / attendanceCount.count).ToString("F2")
            : "0.00";

                counts.Add(count);
            });

            var _result = new
            {
                months = allMonths.Select(x => x.month).ToList(),
                counts = counts
            };

            return _result;
        }


        [HttpGet]
        [Route("GetAttendanceStatisticsForGroups/{studentId}")]
        public async Task<IActionResult> GetAttendanceStatisticsForGroups(int studentId,[FromQuery]int[] groups)
        {
            
            var result=new List<object>();
            foreach (int id in groups)
            {
                var data = await GetAttendanceStatistics2(id, studentId);
                result.Add(data);
            }
            return Ok(result);
        }

        [Obsolete]
        [HttpGet]
        [Route("GetAttendanceStatisticsPerGroup/{studentId}")]
        public async Task<IActionResult> GetAttendanceStatisticsPerGroup(int studentId)
        {

            var data = await db.Attendances
                .Where(f => f.StudentId == studentId)
                .Include(x => x.Group)
                .GroupBy(f => new
                {
                    f.Date.Month,
                    f.GroupId,
                    f.Group.Name
                })
                .Select(x => new
                {
                    month = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Key.Month),
                    month_int = x.Key.Month,
                    groupId=x.Key.GroupId,
                    groupName=x.Key.Name,
                    count = x.Count()
                })
                .ToListAsync();

            var months = new List<string>();
            var groups=new List<Group>();

            foreach (var d in data.OrderBy(f => f.month_int))
            {
                if (!months.Contains(d.month))
                    months.Add(d.month);

                if (groups.Find(f=>f.Id==d.groupId)==null)
                    groups.Add(db.Groups.FirstOrDefault(f=>f.Id==d.groupId));
            }

            var datasets = new List<dynamic>();

            groups.ForEach(g =>
            {
                var arrAttendance = new float[months.Count];
                for (int i = 0; i < months.Count; i++)
                {
                    int month = Convert.ToDateTime(months[i] + " 01, 1900").Month;
                    float present = db.Attendances.Where(f => f.Date.Month == month && g.Id == f.GroupId && f.StudentId == studentId).Count();
                    float percentPresent = db.Attendances.Where(f => f.Date.Month == month &&  g.Id== f.GroupId && f.StudentId == studentId && f.Status).Count();
                    float totalPresentByMonth = percentPresent / present;
                    arrAttendance[i] = totalPresentByMonth;
                }
                datasets.Add(new
                {
                    label = g.Name,
                data = arrAttendance
            }) ;
            });



            //foreach (var d in data.OrderBy(f => f.month_int))
            //{
            //    float percentPresent = db.Attendances.Where(f => f.Date.Month==d.month_int && d.groupId == f.GroupId && f.StudentId == studentId && f.Status).Count();
            //    float totalPresentByMonth=percentPresent/d.count;
            //    var currentdataSet = datasets.FirstOrDefault(f =>f.label==d.groupName);
            //    if (currentdataSet!=null)
            //    {
            //        currentdataSet.data.Add(totalPresentByMonth);
            //    }
            //    else
            //    {
            //        dynamic dataset = new ExpandoObject();
            //        dataset.label = d.groupName;
            //        dataset.data = new List<float>();
            //        dataset.data.Add(totalPresentByMonth);
            //        datasets.Add(dataset);
            //    }
            //}

            var _result = new
            {
                months = months,
                datasets = datasets
            };

            return Ok(_result);
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
            int id = int.Parse(this.jwt.GetTokenClaims());
            var user = db.Users.FirstOrDefault(f=>f.Id==id);

            if (user == null)
                return BadRequest();
            if(user.RoleId==1)
            {
                var result_ = db.Groups
               .OrderByDescending(g => g.OpeningDate)
               .Select(f => new
               {
                   id = f.Id,
                   name = f.Name
               }).ToList();

                return Ok(result_);
            }

            var result = db.Groups
                .Include(f=>f.UserGroups)
                .Where(g => g.UserGroups.Any(f=>f.UserId==id))
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
