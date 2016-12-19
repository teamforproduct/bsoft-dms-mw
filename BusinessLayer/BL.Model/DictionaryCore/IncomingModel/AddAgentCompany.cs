using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Контрагент - юридическое лицо
    /// </summary>
    public class AddAgentCompany: AddAgent
    {
        /// <summary>
        /// Полное наименование
        /// </summary>
        [Required]
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
        [Required]
        public bool IsActive { get; set; }
        
    }
}
