using System.Collections.Generic;
using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// Контрагент - юридическое лицо
    /// </summary>
    public class InternalDictionaryAgentCompany : InternalDictionaryAgent
    {

        public InternalDictionaryAgentCompany()
        { }

        public InternalDictionaryAgentCompany(AddAgentCompany model)
        {
            SetInternalDictionaryAgentCompany(model);
        }

        public InternalDictionaryAgentCompany(ModifyAgentCompany model)
        {
            Id = model.Id;
            SetInternalDictionaryAgentCompany(model);
        }

        private void SetInternalDictionaryAgentCompany(AddAgentCompany model)
        {
            FullName = model.FullName;
            Name = model.Name;
            TaxCode = model.TaxCode;
            Description = model.Description;
            OKPOCode = model.OKPOCode;
            VATCode = model.VATCode;
            IsActive = model.IsActive;
        }
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

    }
}
