using BL.Database.DBModel.System;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionarySettingTypes
    {
        public DictionarySettingTypes()
        {
            this.Settings = new HashSet<SystemSettings>();
        }
        public int Id { get; set; }

        [MaxLength(400)]
        [Index("IX_Code", 1, IsUnique = true)]
        public string Code { get; set; }

        [MaxLength(400)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        public int Order { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("SettingTypeId")]
        public virtual ICollection<SystemSettings> Settings { get; set; }
    }
}
