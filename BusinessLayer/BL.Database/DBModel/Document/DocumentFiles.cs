using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.Document
{
    public class DocumentFiles
    {
        public int Id { get; set; }
        [Index("IX_DocumentNameExtensionVersion", 1, IsUnique = true)]
        [Index("IX_DocumentOrderNumberVersion", 1, IsUnique = true)]
        public int DocumentId { get; set; }
        [MaxLength(2000)]
        [Index("IX_DocumentNameExtensionVersion", 2, IsUnique = true)]
        public string Name { get; set; }
        [Index("IX_DocumentOrderNumberVersion", 2, IsUnique = true)]
        public int OrderNumber { get; set; }
        [Index("IX_DocumentNameExtensionVersion", 4, IsUnique = true)]
        [Index("IX_DocumentOrderNumberVersion", 3, IsUnique = true)]
        public int Version { get; set; }
        [MaxLength(2000)]
        [Index("IX_DocumentNameExtensionVersion", 3, IsUnique = true)]
        public string Extension { get; set; }
        [MaxLength(2000)]
        public string FileType { get; set; }
        public long FileSize { get; set; }
        public DateTime Date { get; set; }
        public string Content { get; set; }
        public bool IsAdditional { get; set; }
        public bool IsDeleted { get; set; }
        public bool? IsWorkedOut { get; set; }
        [MaxLength(2000)]
        public string Hash { get; set; }
        [MaxLength(2000)]
        public string Description { get; set; }
        public bool IsMainVersion { get; set; }
        public int ExecutorPositionId { get; set; }
        [Column("ExecutorPositionExeAgentId")]
        public int ExecutorPositionExecutorAgentId { get; set; }

        public int LastChangeUserId { get; set; }

        [Index("IX_LastChangeDate")]
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("ExecutorPositionId")]
        public virtual DictionaryPositions ExecutorPosition { get; set; }
        [ForeignKey("ExecutorPositionExecutorAgentId")]
        public virtual DictionaryAgents ExecutorPositionExecutorAgent { get; set; }
    }
}
