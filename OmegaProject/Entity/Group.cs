﻿using OmegaProject.Entity;
using System;
using System.Collections.Generic;

namespace OmegaProject.DTO
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; }
        public DateTime? OpeningDate { get; set; } 
        public DateTime? ClosingDate { get; set; }
        public string ImageProfile { get; internal set; }

        public virtual Course Course { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }

        public virtual ICollection<GroupMessage> GroupMessages { get; set; }
    }
}
