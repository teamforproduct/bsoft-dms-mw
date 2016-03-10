using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;

namespace BL.Model.DictionaryCore.FrontModel
{
    /// <summary>
    /// Контакт агента
    /// </summary>
    public class FrontDictionaryContact
    {
        /// <summary>
        /// ИД
        /// </summary>
        public EnumContactTypes Id { get; set; }
        /// <summary>
        /// Тип контакта
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
    }
}
