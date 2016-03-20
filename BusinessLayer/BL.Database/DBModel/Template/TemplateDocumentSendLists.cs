using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocumentSendLists
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int SendTypeId { get; set; }
        public int? SourcePositionId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public Nullable<int> TaskId { get; set; }
        public string Description { get; set; }
        public int Stage { get; set; }
        public int? DueDay { get; set; }
        public int AccessLevelId { get; set; }
        public bool IsAddControl { get; set; }
        public bool IsAvailableWithinTask { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual TemplateDocuments Document { get; set; }
        [ForeignKey("SendTypeId")]
        public virtual DictionarySendTypes SendType { get; set; }
        [ForeignKey("SourcePositionId")]
        public virtual DictionaryAgents SourceAgent { get; set; }
        [ForeignKey("TargetPositionId")]
        public virtual DictionaryPositions TargetPosition { get; set; }
        [ForeignKey("TargetAgentId")]
        public virtual DictionaryAgents TargetAgent { get; set; }
        [ForeignKey("AccessLevelId")]
        public virtual AdminAccessLevels AccessLevel { get; set; }
        [ForeignKey("TaskId")]
        public virtual TemplateDocumentTasks Task { get; set; }

    }
}
