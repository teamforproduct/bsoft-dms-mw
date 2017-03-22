using BL.Model.Common;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр юридических лиц
    /// </summary>
    public class FilterDictionaryAgentCompany : BaseFilterNameIsActive
    {
        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get; set; }

        
        /// <summary>
        /// ИНН
        /// </summary>
        public string TaxCode { get; set; }
        [IgnoreDataMember]
        public string TaxCodeExact { get; set; }
        /// <summary>
        /// ОКПО
        /// </summary>
        public string OKPOCode { get; set; }
        [IgnoreDataMember]
        public string OKPOCodeExact { get; set; }
        /// <summary>
        /// Номер свидетельства НДС
        /// </summary>
        public string VATCode { get; set; }
        [IgnoreDataMember]
        public string VATCodeExact { get; set; }

    }
}
