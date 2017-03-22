using BL.Model.Common;

namespace BL.Model.DictionaryCore.FilterModel
{
    public class FilterCustomDictionaryType : BaseFilterCodeName
    {
        /// <summary>
        /// Сужение по активности элементов
        /// </summary>
        public bool? IsActive { get; set; }
    }
}
