using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentFiles
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Name { get; set; }
        public string Extention { get; set; }
        public System.DateTime Date { get; set; }
        public byte[] Content { get; set; }
        public bool IsAdditional { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }

        public virtual Documents Document { get; set; }
    }
}
