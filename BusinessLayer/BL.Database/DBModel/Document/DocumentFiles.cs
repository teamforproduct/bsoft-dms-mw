using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BL.Database.DBModel.Dictionary;

namespace BL.Database.DBModel.Document
{
    public class DocumentFiles
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        [MaxLength(2000)]
        public string Name { get; set; }
        public int OrderNumber { get; set; }
        public int Version { get; set; }
        [MaxLength(2000)]
        public string Extension { get; set; }
        [MaxLength(2000)]
        public string FileType { get; set; }
        public int FileSize { get; set; }
        public DateTime Date { get; set; }
        public byte[] Content { get; set; }
        public bool IsAdditional { get; set; }
        [MaxLength(2000)]
        public string Hash { get; set; }
        public int ExecutorPositionId { get; set; }
        [Column("ExecutorPositionExeAgentId")]
        public int ExecutorPositionExecutorAgentId { get; set; }

        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }
        [ForeignKey("DocumentId")]
        public virtual Documents Document { get; set; }
        [ForeignKey("ExecutorPositionId")]
        public virtual DictionaryPositions ExecutorPosition { get; set; }
        [ForeignKey("ExecutorPositionExecutorAgentId")]
        public virtual DictionaryAgents ExecutorPositionExecutorAgent { get; set; }
    }
}
