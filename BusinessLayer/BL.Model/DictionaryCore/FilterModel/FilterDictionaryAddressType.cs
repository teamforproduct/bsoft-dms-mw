using BL.Model.Common;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтр типов адресов
    /// </summary>
    public class FilterDictionaryAddressType : BaseFilterCodeNameIsActive
    {
        [IgnoreDataMember]
        public string CodeName { get; set; }

    }
}
