using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BL.Database.DBModel.Dictionary
{
    public class DictionaryStandartSendLists
    {
        public DictionaryStandartSendLists()
        {
            this.StandartSendListContents = new HashSet<DictionaryStandartSendListContents>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int PositionId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public virtual ICollection<DictionaryStandartSendListContents> StandartSendListContents { get; set; }
        [ForeignKey("PositionId")]
        public virtual DictionaryPositions Position { get; set; }
    }
}
