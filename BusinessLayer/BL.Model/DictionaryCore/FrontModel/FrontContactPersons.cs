using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.Model.Enums;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FrontModel
{

    /// <summary>
    /// Контактные лица
    /// </summary>
    public class FrontContactPersons
    {
        /// <summary>
        /// ИД
        /// </summary>
        public int Id { get; set; }
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
        /// Список контактов контрагента
        /// </summary>
        public IEnumerable<FrontDictionaryContact> Contacts { get; set; }

    }
}
