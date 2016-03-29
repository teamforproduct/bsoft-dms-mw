using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр юридических лиц
    /// </summary>
    public class FilterDictionaryAgentCompany : DictionaryBaseFilterParms
    {
         
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
        /// Первая буква наименования
        /// </summary>
        public char FirstChar { get; set; }
    }
}
