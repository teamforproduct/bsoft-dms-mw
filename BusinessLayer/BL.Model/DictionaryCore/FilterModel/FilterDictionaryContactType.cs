using BL.Model.Common;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр типов контактов
    /// </summary>
    public class FilterDictionaryContactType : BaseFilterCodeNameIsActive
    {

        [IgnoreDataMember]
        public string CodeName { get; set; }
    }
}
