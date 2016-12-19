using BL.Model.Common;
using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Карточка элемента из справочника "Компании". 
    /// </summary>
    public class FrontDictionaryAgentClientCompany : ModifyAgentClientCompany
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

    }
}