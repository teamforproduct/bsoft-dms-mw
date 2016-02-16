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
        public int SourcePositionId { get; set; }
        public int TargetPositionId { get; set; }
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
