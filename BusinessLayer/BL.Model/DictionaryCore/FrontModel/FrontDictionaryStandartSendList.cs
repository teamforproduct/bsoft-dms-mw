using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    public class FrontDictionaryStandartSendList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? PositionId { get; set; }
        public int LastChangeUserId { get; set; }
        public DateTime LastChangeDate { get; set; }

        public string PositionName { get; set; }

        public virtual IEnumerable<FrontDictionaryStandartSendListContent> StandartSendListContents { get; set; }
    }
}
