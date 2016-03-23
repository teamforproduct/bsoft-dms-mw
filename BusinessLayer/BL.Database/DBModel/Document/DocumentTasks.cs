using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        }

        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int PositionId { get; set; }
        public int PositionExecutorAgentId { get; set; }
        public int AgentId { get; set; }
        [MaxLength(2000)]
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

        public virtual ICollection<DocumentEvents> Events { get; set; }

        public virtual ICollection<DocumentSendLists> SendLists { get; set; }
    }
}
