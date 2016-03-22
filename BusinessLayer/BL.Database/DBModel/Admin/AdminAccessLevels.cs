using System;
using System.ComponentModel.DataAnnotations;

namespace BL.Database.DBModel.Admin
{
    public class AdminAccessLevels
    {
        public int Id { get; set; }
        [MaxLength(2000)]
        public string Code { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
