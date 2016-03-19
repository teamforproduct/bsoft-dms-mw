using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;
using BL.Model.Common;

namespace BL.Model.DictionaryCore.InternalModel
{
    /// <summary>
    /// контакт контрагента
    /// </summary>
    public class InternalDictionaryContact : LastChangeInfo
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ID агента
        /// </summary>
        public int AgentId { get; set; }
        /// <summary>
        /// Тип контакта
        /// </summary>
        public int ContactTypeId { get; set; }
        /// <summary>
        /// Значение
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Признак активности
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }
    }
}
