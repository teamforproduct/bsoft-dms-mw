using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - юридическое лицо
    /// </summary>
    public class FrontDictionaryAgentCompany : FrontDictionaryAgent
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Полное наименование
        /// </summary>
        public string FullName { get; set; }
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
        /// Комментарии
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Краткое наименование (поле Name из таблицы DictionaryAgents)
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// Список контактных лиц
        /// </summary>
        public IEnumerable<FrontDictionaryAgentPerson> ContactsPersons { get; set; }


    }
}
