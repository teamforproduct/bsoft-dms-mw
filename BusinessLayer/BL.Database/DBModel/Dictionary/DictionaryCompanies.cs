﻿using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.Dictionary
{

    public class DictionaryCompanies
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }

}
