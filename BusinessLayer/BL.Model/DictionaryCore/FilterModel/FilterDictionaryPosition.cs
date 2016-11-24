using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BL.Model.DictionaryCore.FilterModel
{
    /// <summary>
    /// Фильтры FilterDictionaryPosition
    /// </summary>
    public class FilterDictionaryPosition : DictionaryBaseFilterParameters
    {
 
        /// <summary>
        /// Сужение по полному наименованию подразделений
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// По отделам
        /// </summary>
        public List<int> DepartmentIDs { get; set; }

        /// <summary>
        /// по вышестоящей должности
        /// </summary>
        [IgnoreDataMember]
        public List<int> DarentIDs { get; set; }

        /// <summary>
        /// Массив ИД документов для поиска корреспондентов в событиях
        /// </summary>
        public List<int> DocumentIDs { get; set; }
        /// <summary>
        /// Массив ИД должностей для проверки субординации
        /// </summary>
        public List<int> SubordinatedPositions { get; set; }

        /// <summary>
        /// Тип субординации
        /// </summary>
        public int? SubordinatedTypeId { get; set; }

        /// <summary>
        /// Должности, которым назначены указанные роли
        /// </summary>
        public List<int> RoleIDs { get; set; }

        /// <summary>
        ///  Ниже по списку
        /// </summary>
        public int? OrderMore { get; set; }

        /// <summary>
        /// Выше по списку
        /// </summary>
        public int? OrderLess { get; set; }

        /// <summary>
        /// Номер в списке
        /// </summary>
        public List<int> Orders { get; set; }
    }
}
