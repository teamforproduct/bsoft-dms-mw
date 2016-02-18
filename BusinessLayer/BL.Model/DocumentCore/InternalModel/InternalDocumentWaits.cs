using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentWaits
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public Nullable<int> ParentId { get; set; }
        public int OnEventId { get; set; }
        public Nullable<int> OffEventId { get; set; }
        public Nullable<int> ResultTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> DueDate { get; set; }
        public Nullable<DateTime> AttentionDate { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
