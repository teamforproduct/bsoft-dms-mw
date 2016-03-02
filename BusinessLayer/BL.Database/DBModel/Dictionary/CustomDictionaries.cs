﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Dictionary
{
    public class CustomDictionaries
    {
        public int Id { get; set; }
        public int DictionaryTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DictionaryTypeId")]
        public virtual CustomDictionaryTypes CustomDictionaryType { get; set; }
    }
}
