using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.System
{
    public class SystemSettings
    {
        [Key]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}