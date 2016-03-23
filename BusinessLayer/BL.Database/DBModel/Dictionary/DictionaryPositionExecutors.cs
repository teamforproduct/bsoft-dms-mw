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
        public int AgentId { get; set; }
        public int PositionId { get; set; }
        public int PositionExecutorTypeId { get; set; }
        public int AccessLevelId { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
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
