using BL.Database.DBModel.Admin;
using BL.Database.DBModel.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Database.DBModel.Document
{
    public class DocumentAccesses
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int PositionId { get; set; }
        public int AccessLevelId { get; set; }
        public bool IsInWork { get; set; }
        public int LastChangeUserId { get; set; }
        public System.DateTime LastChangeDate { get; set; }

        public virtual Documents Document { get; set; }
        public virtual DictionaryPositions Position { get; set; }
        public virtual AdminAccessLevels AccessLevel { get; set; }
    }
}
