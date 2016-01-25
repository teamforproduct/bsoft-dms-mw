using System;

namespace BL.Database.DBModel.Document
{
    public class DocumentFiles
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public string Extention { get; set; }
        public DateTime Date { get; set; }
        public byte[] Content { get; set; }
        public bool IsAdditional { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual Documents Document { get; set; }
    }
}
