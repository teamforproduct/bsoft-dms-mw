﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionarySubscriptionStates
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        public bool IsSuccess { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
