﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentFiles
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public int OrderNumber { get; set; }
        public int Version { get; set; }
        public string Extension { get; set; }
        public DateTime Date { get; set; }
        public byte[] Content { get; set; }
        public bool IsAdditional { get; set; }
        public string Hash { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
    }
}
