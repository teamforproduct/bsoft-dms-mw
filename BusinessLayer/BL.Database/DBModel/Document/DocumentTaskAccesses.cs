using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public partial class DocumentTaskAccesses
    {
        public int Id { get; set; }
        [Index("IX_TaskPosition", 1, IsUnique = true)]
        [Index("IX_PositionTask", 2, IsUnique = true)]
        public int TaskId { get; set; }
        [Index("IX_TaskPosition", 2, IsUnique = true)]
        [Index("IX_PositionTask", 1, IsUnique = true)]
        public int PositionId { get; set; }
        [ForeignKey("TaskId")]
        public virtual DocumentTasks Task { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
    }
}
