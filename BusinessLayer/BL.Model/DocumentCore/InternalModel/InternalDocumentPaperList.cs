using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentPaperList : LastChangeInfo
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public IEnumerable<InternalDocumentEvent> Events { get; set; }
        public int? SourcePositionId { get; set; }
        public int? TargetPositionId { get; set; }
        public int? TargetAgentId { get; set; }
    }
}
