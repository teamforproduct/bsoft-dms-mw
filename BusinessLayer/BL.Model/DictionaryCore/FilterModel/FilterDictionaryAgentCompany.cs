using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр юридических лиц
    /// </summary>
    public class FilterDictionaryAgentCompany
    {
        /// <summary>
        /// Массив ИД компаний
        /// </summary>
        public List<int> CompanyId { get; set; }
        /// <summary>
        /// наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxCode { get; set; }
        /// <summary>
        /// ОКПО
        /// </summary>
        public string OKPOCode { get; set; }
        /// <summary>
        /// Номер свидетельства НДС
        /// </summary>
        public string VATCode { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool? IsActive { get; set; }
        /// <summary>
        ///  игнорировать при поиске
        /// </summary>
        public List<int> NotContainsId { get; set; }
        /// <summary>
        /// Первая буква наименования
        /// </summary>
        public char FirstChar { get; set; }
    }
}
