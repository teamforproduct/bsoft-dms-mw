using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    [Table("DicPositionExecutorTypes")]
    public class DictionaryPositionExecutorTypes
    {
        public DictionaryPositionExecutorTypes()
        {
            this.PositionExecutors = new HashSet<DictionaryPositionExecutors>();
        }

        public int Id { get; set; }
        [MaxLength(400)]
        [Index("IX_Code", 1, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(400)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        [ForeignKey("PositionExecutorTypeId")]
        public virtual ICollection<DictionaryPositionExecutors> PositionExecutors { get; set; }
    }
}
