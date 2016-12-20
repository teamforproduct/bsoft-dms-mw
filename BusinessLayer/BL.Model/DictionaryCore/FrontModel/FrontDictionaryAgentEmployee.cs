using BL.Model.DictionaryCore.IncomingModel;
using BL.Model.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контрагент - сотрудник
    /// </summary>
    public class FrontDictionaryAgentEmployee: ModifyAgentEmployee
    {

        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Наименование языка
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        /// Код языка
        /// </summary>
        public string LanguageCode { get; set; }
    }
}
