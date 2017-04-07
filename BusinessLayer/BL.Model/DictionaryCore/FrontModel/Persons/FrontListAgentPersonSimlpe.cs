using BL.Model.DictionaryCore.IncomingModel;
using System.Collections.Generic;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Контактные лица
    /// </summary>
    public class FrontContactPersons : ModifyAgentContactPerson
    {
        /// <summary>
        /// Краткое название/имя (отображается в интерфейсе как основное)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Список контактов контрагента
        /// </summary>
        public IEnumerable<FrontDictionaryAgentContact> Contacts { get; set; }

    }
}
