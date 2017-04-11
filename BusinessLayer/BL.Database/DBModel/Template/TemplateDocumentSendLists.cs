using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Template
{
    public class TemplateDocumentSendLists
    {
        public TemplateDocumentSendLists()
        {
            this.AccessGroups = new HashSet<TemplateDocumentSendListAccessGroups>();
        }
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int SendTypeId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public Nullable<int> TaskId { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public int? StageTypeId { get; set; }
        public int Stage { get; set; }
        public int? DueDay { get; set; }
        public int AccessLevelId { get; set; }
        public bool IsWorkGroup { get; set; }
        public bool IsAddControl { get; set; }
        [MaxLength(2000)]
        public string SelfDescription { get; set; }
        public int? SelfDueDay { get; set; }
        public int? SelfAttentionDay { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual TemplateDocuments Document { get; set; }
        [ForeignKey("StageTypeId")]
        public virtual DictionaryStageTypes StageType { get; set; }
        [ForeignKey("SendTypeId")]
        public virtual DictionarySendTypes SendType { get; set; }
        [ForeignKey("TargetPositionId")]
        public virtual DictionaryPositions TargetPosition { get; set; }
        [ForeignKey("TargetAgentId")]
        public virtual DictionaryAgents TargetAgent { get; set; }
        [ForeignKey("AccessLevelId")]
        public virtual AdminAccessLevels AccessLevel { get; set; }
        [ForeignKey("TaskId")]
        public virtual TemplateDocumentTasks Task { get; set; }
        [ForeignKey("SendListId")]
        public virtual ICollection<TemplateDocumentSendListAccessGroups> AccessGroups { get; set; }

    }
}
