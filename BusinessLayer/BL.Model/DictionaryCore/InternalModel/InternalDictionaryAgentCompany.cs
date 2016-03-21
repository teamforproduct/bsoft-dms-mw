using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Контрагент - юридическое лицо
    /// </summary>
    public class InternalDictionaryAgentCompany : LastChangeInfo
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
        /// Краткое наименование
        /// </summary>
        public string ShortName { get; set; }
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
        /// список контактов
        /// </summary>
        public IEnumerable<int> ContactsPersonsId { get; set; }

    }
}
