using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Admin
{
    public class AdminSubordinations
    {
        public int Id { get; set; }
        [Index("IX_SourceTargetType", 1, IsUnique = true)]
        public int SourcePositionId { get; set; }
        [Index("IX_SourceTargetType", 2, IsUnique = true)]
        [Index("IX_TargetPositionId", 1)]
        public int TargetPositionId { get; set; }
        [Index("IX_SourceTargetType", 3, IsUnique = true)]
        [Index("IX_SubordinationTypeId", 1)]
        public int SubordinationTypeId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("SourcePositionId")]
        public virtual DictionaryPositions SourcePosition { get; set; }
        [ForeignKey("TargetPositionId")]
        public virtual DictionaryPositions TargetPosition { get; set; }
        [ForeignKey("SubordinationTypeId")]
        public virtual DictionarySubordinationTypes SubordinationType { get; set; }
    }
}
