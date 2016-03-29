using System.Collections.Generic;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Контрагент - юридическое лицо
    /// </summary>
    public class InternalDictionaryAgentCompany : LastChangeInfo
    {

        public InternalDictionaryAgentCompany()
        { }

        public InternalDictionaryAgentCompany(ModifyDictionaryAgentCompany model)
        {
            Id = model.Id;
            FullName = model.FullName;
            TaxCode = model.TaxCode;
            Description = model.Description;
            OKPOCode = model.OKPOCode;
            VATCode = model.VATCode;
            ContactsPersonsId = model.ContactsPersonsId;
            IsActive = model.IsActive;
        }
        
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
