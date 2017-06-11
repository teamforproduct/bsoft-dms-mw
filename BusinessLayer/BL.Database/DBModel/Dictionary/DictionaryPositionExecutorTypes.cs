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

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }

        [MaxLength(400)]
        public string Suffix { get; set; }
        public bool IsActive { get; set; }

        [ForeignKey("PositionExecutorTypeId")]
        public virtual ICollection<DictionaryPositionExecutors> PositionExecutors { get; set; }
    }
}
