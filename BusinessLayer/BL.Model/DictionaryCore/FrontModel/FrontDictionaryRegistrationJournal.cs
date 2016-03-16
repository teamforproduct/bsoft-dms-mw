using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Элемент справочника "Журналы регистрации". 
    /// </summary>
    public class FrontDictionaryRegistrationJournal : ModifyDictionaryRegistrationJournal
    {
        /// <summary>
        /// ID
        /// </summary>
        public new int Id { get; set; }

        /// <summary>
        /// Подразделение, к которому приписан журнал регистрации
        /// </summary>
        public string DepartmentName { get; set; }

    }
}