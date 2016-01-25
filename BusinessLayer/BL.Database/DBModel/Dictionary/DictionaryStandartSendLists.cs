using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public System.DateTime LastChangeDate { get; set; }

        public virtual ICollection<DictionaryStandartSendListContents> StandartSendListContents { get; set; }
        public virtual DictionaryPositions Position { get; set; }
    }
}
