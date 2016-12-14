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
    public class FrontDictionaryAgentEmployee
    {

        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Полное имя
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// ФИО, Название, ...
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Дополнительная информация
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Признак активности
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Список адресов контрагента
        /// </summary>
        public IEnumerable<FrontDictionaryPositionExecutor> PositionExecutors { get; set; }

    }
}
