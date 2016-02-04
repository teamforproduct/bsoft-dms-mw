﻿using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int? OrderNumber { get; set; }
        public int SendTypeId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public int? DueDay { get; set; }
        public int AccessLevelId { get; set; }
        public bool IsInitial { get; set; }
        public Nullable<int> EventId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("SendTypeId")]
        public virtual DictionarySendTypes SendType { get; set; }
        [ForeignKey("TargetPositionId")]
        public virtual DictionaryPositions TargetPosition { get; set; }
        [ForeignKey("AccessLevelId")]
        public virtual AdminAccessLevels AccessLevel { get; set; }
        [ForeignKey("EventId")]
        public virtual DocumentEvents Event { get; set; }
    }
}
