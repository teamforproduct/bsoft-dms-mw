using System;

namespace BL.Model.DocumentAdditional
{
    public class DocumentAttachedFile
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int OrderInDocument { get; set; }
        public int Version { get; set; }
        public byte[] FileData { get; set; }
        public string Name { get; set; }
        public string Extension { get; set; }
        public string FileType { get; set; }
        public bool IsAdditional { get; set; }
        public DateTime Date { get; set; }
        public string Hash { get; set; }
        public int LastChangeUserId { get; set; }
        public string LastChangeUserName { get; set; }
        public DateTime LastChangeDate { get; set; }
        public bool WasChangedExternal { get; set; }
    }
}