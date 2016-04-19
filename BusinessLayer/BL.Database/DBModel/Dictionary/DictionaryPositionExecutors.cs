using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Database.DBModel.Admin;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryPositionExecutors
    {
        public int Id { get; set; }
        [Index("IX_PositionAgentStartDate", 2, IsUnique = true)]
        [Index("IX_AgentId", 1)]
        public int AgentId { get; set; }
        [Index("IX_PositionAgentStartDate", 1, IsUnique = true)]
        public int PositionId { get; set; }
        public int PositionExecutorTypeId { get; set; }
        public int AccessLevelId { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        [Index("IX_PositionAgentStartDate", 3, IsUnique = true)]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
        [ForeignKey("AgentId")]
        public virtual DictionaryAgents Agent { get; set; }
        [ForeignKey("PositionExecutorTypeId")]
        public virtual DictionaryPositionExecutorTypes PositionExecutorType { get; set; }
        [ForeignKey("AccessLevelId")]
        public virtual AdminAccessLevels AccessLevel { get; set; }
    }
}
