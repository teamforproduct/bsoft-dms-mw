using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.IncomingModel
{
    /// <summary>
    /// Типы контактов
    /// </summary>
    public class ModifyDictionaryContactType
    {
        /// <summary>
        /// ID
        /// </summary>
        [IgnoreDataMember]
        public int Id { get; set; }

        /// <summary>
        /// Название типа контакта
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Маска ввода
        /// </summary>
        public string InputMask { get; set; }

        /// <summary>
        /// Краткий идентификатор
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
    }
}
