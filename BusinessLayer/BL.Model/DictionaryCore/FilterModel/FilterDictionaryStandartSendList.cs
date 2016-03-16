using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterDictionaryStandartSendList
    {
        public List<int> StandartSendListId { get; set; }
        public List<int?> PositionId { get; set; }
        public List<int> NotContainsId { get; set; }
    }
}
