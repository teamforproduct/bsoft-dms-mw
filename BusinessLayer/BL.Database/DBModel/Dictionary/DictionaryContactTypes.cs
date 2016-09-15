using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryContactTypes
    {
        public int Id { get; set; }
        [Index("IX_Name", 2, IsUnique = true)]
        [Index("IX_Code", 2, IsUnique = true)]
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }
        [MaxLength(400)]
        [Index("IX_Code", 1, IsUnique = true)]
        public string Code { get; set; }
        [MaxLength(400)]
        [Index("IX_Name", 1, IsUnique = true)]
        public string Name { get; set; }
        [MaxLength(2000)]
        public string SpecCode { get; set; }
        [MaxLength(2000)]
        public string InputMask { get; set; }
        public bool IsActive { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
    }
}
