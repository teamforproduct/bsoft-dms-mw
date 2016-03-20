using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;

namespace BL.Model.DocumentCore.InternalModel
{
    public class InternalDocumentTask : LastChangeInfo
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int PositionId { get; set; }
        public int PositionExecutorAgentId { get; set; }
        public int AgentId { get; set; }
        public string Task { get; set; }
        public string Description { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
