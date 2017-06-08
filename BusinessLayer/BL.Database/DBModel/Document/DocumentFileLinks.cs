using BL.Database.DBModel.Dictionary;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Document
{
    public class DocumentFileLinks
    {
        public int Id { get; set; }
        [Index("IX_ClientId", 1)]
        public int ClientId { get; set; }

        [Index("IX_FileEvent", 1, IsUnique = true)]
        public int FileId { get; set; }
        [Index("IX_FileEvent", 2, IsUnique = true)]
        [Index("IX_Event")]
        public int? EventId { get; set; }

        [ForeignKey("FileId")]
        public virtual DocumentFiles File { get; set; }
        [ForeignKey("EventId")]
        public virtual DocumentEvents Event { get; set; }

    }
}
