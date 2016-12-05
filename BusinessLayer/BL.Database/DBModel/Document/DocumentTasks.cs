﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.Document
{
    public class DocumentTasks
    {

        public DocumentTasks()
        {
            this.Events = new HashSet<DocumentEvents>();
            this.SendLists = new HashSet<DocumentSendLists>();
            this.TaskAccesses = new HashSet<DocumentTaskAccesses>();
        }

        public int Id { get; set; }
        [Index("IX_DocumentTask", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        public int PositionId { get; set; }
        public int PositionExecutorAgentId { get; set; }
        public int? PositionExecutorTypeId { get; set; }
        public int AgentId { get; set; }
        [MaxLength(400)]
        [Index("IX_DocumentTask", 2, IsUnique = true)]
        public string Task { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("PositionExecutorAgentId")]
        public virtual DictionaryAgents PositionExecutorAgent { get; set; }
        [ForeignKey("PositionExecutorTypeId")]
        public virtual DictionaryPositionExecutorTypes PositionExecutorType { get; set; }

        public virtual ICollection<DocumentEvents> Events { get; set; }

        public virtual ICollection<DocumentSendLists> SendLists { get; set; }

        public virtual ICollection<DocumentTaskAccesses> TaskAccesses { get; set; }
    }
}
