using MailKit.Net.Smtp;
using MimeKit;
using OmegaProject.Entity;
using System;
using System.Collections.Generic;
using System.IO;

namespace OmegaProject.services
{
   
    public partial class MyTools
    {
        public static void RemoveFilesOfStudents(List<HomeWorkStudent> homeWorkStudent,string mainRoot)
        {
            var target = mainRoot;
            homeWorkStudent.ForEach(hws =>
            {
                string path = Path.Combine(mainRoot, hws.StudentId+"",hws.HomeWorkId+"");
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            });
        }
    }
}
