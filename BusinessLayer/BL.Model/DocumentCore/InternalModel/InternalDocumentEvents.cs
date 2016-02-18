using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentEvents
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int EventTypeId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int SourcePositionId { get; set; }
        public int SourceAgentId { get; set; }
        public Nullable<int> TargetPositionId { get; set; }
        public Nullable<int> TargetAgentId { get; set; }
        public Nullable<DateTime> ReadDate { get; set; }
        public Nullable<int> ReadAgentId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
