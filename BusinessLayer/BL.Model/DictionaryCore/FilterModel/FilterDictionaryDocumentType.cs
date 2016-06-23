using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// фильтр типов документов
    /// </summary>
    public class FilterDictionaryDocumentType : DictionaryBaseFilterParameters
    {
        public string NameExact { get; set; }
    }
}
