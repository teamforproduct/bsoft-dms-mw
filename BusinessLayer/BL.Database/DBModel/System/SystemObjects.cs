﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.System
{

    public  class SystemObjects
    {
        public SystemObjects()
        {
            this.Fields = new HashSet<SystemFields>();
            this.Actions = new HashSet<SystemActions>();
            //this.CalculatedValues = new HashSet<SystemCalculatedValues>();
        }

        public int Id { get; set; }
        [MaxLength(2000)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }

        public virtual ICollection<SystemFields> Fields { get; set; }

        public virtual ICollection<SystemActions> Actions { get; set; }

        //public virtual ICollection<SystemCalculatedValues> CalculatedValues { get; set; }
    }
}
