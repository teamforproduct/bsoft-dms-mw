using BL.Database.DBModel.System;
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

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(400)]
        public string Code { get; set; }

        public int Order { get; set; }

        [ForeignKey("SettingTypeId")]
        public virtual ICollection<SystemSettings> Settings { get; set; }
    }
}
