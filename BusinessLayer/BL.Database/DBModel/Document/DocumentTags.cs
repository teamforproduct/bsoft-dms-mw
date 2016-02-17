﻿using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public partial class DocumentTags
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int TagId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("TagId")]
        public virtual DictionaryTags Tag { get; set; }
    }
}
