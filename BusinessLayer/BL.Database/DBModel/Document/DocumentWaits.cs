using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentWaits
    {
        public DocumentWaits()
        {
            this.ChildWaits = new HashSet<DocumentWaits>();
        }
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public int OnEventId { get; set; }
        public Nullable<int> OffEventId { get; set; }
        public Nullable<int> ResultTypeId { get; set; }

        [Index("IX_DueDate",1)]
        public Nullable<DateTime> DueDate { get; set; }
        public Nullable<DateTime> PlanDueDate { get; set; }
        public Nullable<DateTime> AttentionDate { get; set; }
        [MaxLength(2000)]
        public string TargetDescription { get; set; }
        //        public Nullable<DateTime> TargetAttentionDate { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("ParentId")]
        public virtual DocumentWaits ParentWait { get; set; }
        [ForeignKey("OnEventId")]
        public virtual DocumentEvents OnEvent { get; set; }
        [ForeignKey("OffEventId")]
        public virtual DocumentEvents OffEvent { get; set; }
        [ForeignKey("ResultTypeId")]
        public virtual DictionaryResultTypes ResultType { get; set; }

        public virtual ICollection<DocumentWaits> ChildWaits { get; set; }

    }
}
